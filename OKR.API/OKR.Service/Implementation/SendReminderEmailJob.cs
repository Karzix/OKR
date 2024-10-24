using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OKR.DTO;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using Quartz;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using OKR.Infrastructure.Enum;
using Serilog;

namespace OKR.Service.Implementation
{
    public class SendReminderEmailJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IKeyResultRepository _keyResultRepository;
        private IObjectivesRepository _objectivesRepository;
        private ISidequestsRepository _sidequestsRepository;
        private IUserObjectivesRepository _userObjectivesRepository;
        private IDepartmentObjectivesRepository _departmentObjectivesRepository;
        public SendReminderEmailJob(IServiceScopeFactory serviceScopeFactory)
        {
            //_context = oKRDBContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime thresholdDate = currentDate.AddDays(3);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _keyResultRepository = scope.ServiceProvider.GetRequiredService<IKeyResultRepository>();
                _objectivesRepository = scope.ServiceProvider.GetRequiredService<IObjectivesRepository>();
                _sidequestsRepository = scope.ServiceProvider.GetRequiredService<ISidequestsRepository>();
                _userObjectivesRepository = scope.ServiceProvider.GetRequiredService<IUserObjectivesRepository>();
                _departmentObjectivesRepository = scope.ServiceProvider.GetRequiredService<IDepartmentObjectivesRepository>();
                //var objectives = _objectivesRepository.AsQueryable().ToList();
                var objectives = _objectivesRepository.FindByPredicate(x => x.Deadline.Date <= thresholdDate.Date && x.Deadline.Date >= currentDate.Date)
                    .Select(x=> new ObjectiveDto
                    {
                        CreatedBy = x.CreatedBy,
                        Deadline = x.Deadline,
                        Id = x.Id,
                        Name = x.Name,
                        Point = 0,
                        StartDay = x.StartDay,
                        TargetType = x.TargetType,
                        ListKeyResults = _keyResultRepository.AsQueryable().Where(k=>k.ObjectivesId == x.Id).Select(k=> new KeyResultDto
                        {
                            Active = k.Active,
                            AddedPoints = 0,
                            CurrentPoint = k.CurrentPoint,
                            Deadline = k.Deadline,
                            Description = k.Description,
                            Id = k.Id,
                            MaximunPoint = k.MaximunPoint,
                            Note = "",
                            Unit = k.Unit,
                            Sidequests = _sidequestsRepository.AsQueryable().Where(s=>s.KeyResultsId == k.Id)
                            .Select(s=> new SidequestsDto
                            {
                                Id = s.Id,
                                KeyResultsId = s.Id,
                                Name = s.Name,
                                Status = s.Status,
                             }
                             )
                            .ToList(),
                        }).ToList(),
                    })
                    .ToList();
                var dateNow = DateTime.UtcNow;
                foreach (var objective in objectives)
                {
                    if(objective.Deadline.Value !=  dateNow)
                    {
                        var body = await buildEmailAsync(objective);
                        await SendEmailAsync(objective.CreatedBy, "Reminder: Objective is nearing deadline", body);
                    }
                    else
                    {
                        ChangeStatusFromWorkingToEnd(objective.Id.Value);
                    }
                    
                }
                ChangeStatusFromNotStartedToWorking();
            }
        }
        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                // Tạo một đối tượng MimeMessage
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("kz", "{{ GOOGLE_ACCOUNT }}")); // Địa chỉ email người gửi
                message.To.Add(new MailboxAddress("", recipientEmail)); // Địa chỉ email người nhận
                message.Subject = subject; // Tiêu đề email

                // Cấu hình nội dung email
                message.Body = new TextPart("html")
                {
                    Text = body // Nội dung email
                };

                // Cấu hình SMTP client
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    // Kết nối tới máy chủ SMTP (thay đổi theo dịch vụ bạn sử dụng)
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                    // Thực hiện xác thực
                    await client.AuthenticateAsync("{{ GOOGLE_ACCOUNT }}", "{{ GOOGLE_PASSWORD }}"); // Thay đổi thông tin xác thực

                    // Gửi email
                    await client.SendAsync(message);
                    Console.WriteLine("Email đã được gửi thành công!");

                    // Ngắt kết nối
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private async Task<string> buildEmailAsync(ObjectiveDto objectives)
        {
            try
            {
                //string content = "";
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\OKR.Service\\Template", "Email.html");
                var mailText = await File.ReadAllTextAsync(templatePath);


                mailText = mailText.Replace("{{ObjectiveName}}", objectives.Name);
                mailText = mailText.Replace("{{Dealine}}", objectives.Deadline.Value.AddHours(7).ToShortDateString());

                //var listKeyresuls = _keyResultRepository.AsQueryable().Where(x => x.ObjectivesId == objectives.Id).ToList();
                var keyresults = BuildKeyresult(objectives.ListKeyResults);
                mailText = mailText.Replace("{{ListKeyresult}}", keyresults);
                return mailText;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ex.Message;
            }
        }

        private string BuildKeyresult(List<KeyResultDto> keyResultDto)
        {
            string content = "";
            foreach (var keyResult in keyResultDto)
            {
                content += "<tr>" +
                              $"<td style=\"border: 1px solid #dddddd; padding: 10px;\">{keyResult.Description}</td>" +(
                              keyResult.Unit != TypeUnitKeyResult.Checked ?
                              $"<td style=\"border: 1px solid #dddddd; padding: 10px; text-align: center;\">{keyResult.CurrentPoint}/{keyResult.MaximunPoint}</td>" 
                              : $"<td style=\"border: 1px solid #dddddd; padding: 10px; text-align: center;\">{keyResult.Sidequests.Where(x=>x.Status == true).Count()}/{keyResult.Sidequests.Count}</td>"
                              ) +
                              "<td style=\"border: 1px solid #dddddd; padding: 10px;\">" +
                                  BuildSidequests(keyResult.Sidequests) +
                              "</td>" +
                          "</tr>";
            }
            return content;
        }
        private string BuildSidequests(List<SidequestsDto> sidequestsDtos)
        {
            var content = "";
            if(sidequestsDtos.Count == 0)
            {
                return content;

            }
            content += "<ul style=\"padding: 0; margin: 0; list-style-type: none;\">";
            foreach(var sidequest in sidequestsDtos)
            {
                if(sidequest.Status == true)
                {
                    content += $"<li style=\"margin-bottom: 5px;\">{sidequest.Name}: <span style=\"color: green;\">Done</span></li>";
                }
                else
                {
                    content += $"<li>{sidequest.Name}: <span style=\"color: red;\">Unfinished</span></li>";
                }
            }
            content += "</ul>";
            return content;
        }
        private void ChangeStatusFromWorkingToEnd( Guid ObjectivesID)
        {
            try
            {
                var userObjectivesL = _userObjectivesRepository.AsQueryable().Where(x => x.ObjectivesId == ObjectivesID).ToList();
                var departmentObjectivesL = _departmentObjectivesRepository.AsQueryable().Where(x => x.ObjectivesId == ObjectivesID).ToList();
                if (userObjectivesL.Count() == 0 && departmentObjectivesL.Count == 0)
                {
                    return;
                }
                if (userObjectivesL.Count() != 0)
                {
                    userObjectivesL.ForEach(userObjectives =>
                    {
                        userObjectives.status = StatusObjectives.end;
                        _userObjectivesRepository.Edit(userObjectives);
                    });
                    
                }
                else
                {
                    departmentObjectivesL.ForEach(departmentObjectives =>
                    {
                        departmentObjectives.status = StatusObjectives.end;
                        _departmentObjectivesRepository.Edit(departmentObjectives);
                    });
                    
                }
            }
            catch (Exception ex)
            {
                Log.Error("entity objectives: " + ObjectivesID + " error when the system automatically updates the status: " + ex.Message + " " + ex.StackTrace);
            }
        }

        private void ChangeStatusFromNotStartedToWorking()
        {
            var now = DateTime.UtcNow;
            var list =  _objectivesRepository.AsQueryable()
                .Include(x => x.DepartmentObjectives)
                .Include(x => x.UserObjectives)
                .Where(x => x.StartDay >= now && (
                    x.DepartmentObjectives.Where(dO => dO.ObjectivesId == x.Id && dO.status == StatusObjectives.notStarted).Count() > 0
                    || x.UserObjectives.Where(uo => uo.ObjectivesId == x.Id && uo.status == StatusObjectives.notStarted).Count() > 0
                )).ToList();
            foreach (var item in list)
            {
                var dO = item.DepartmentObjectives;
                var uo = item.UserObjectives;
                if(dO != null && dO.Count != 0)
                {
                    foreach(var item2 in dO)
                    {
                        item2.status = StatusObjectives.working;
                        _departmentObjectivesRepository.Edit(item2);
                    }
                }
                if (uo != null && uo.Count != 0)
                {
                    foreach (var item2 in uo)
                    {
                        item2.status = StatusObjectives.working;
                        _userObjectivesRepository.Edit(item2);
                    }
                }
            }

        }
    }
}
