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
            DateTime targetDate = DateTime.Now.AddDays(3);

            // Xác định quý hiện tại và năm của ngày targetDate
            int month = targetDate.Month;
            int currentQuarter = (month - 1) / 3 + 1;
            int currentYear = targetDate.Year;

            // Xác định quý trước và năm tương ứng
            Quarter previousQuarter;
            int previousYear;

            switch (currentQuarter)
            {
                case 1:
                    previousQuarter = Quarter.Quarter4;
                    previousYear = currentYear - 1;
                    break;
                case 2:
                    previousQuarter = Quarter.Quarter1;
                    previousYear = currentYear;
                    break;
                case 3:
                    previousQuarter = Quarter.Quarter2;
                    previousYear = currentYear;
                    break;
                case 4:
                    previousQuarter = Quarter.Quarter3;
                    previousYear = currentYear;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _keyResultRepository = scope.ServiceProvider.GetRequiredService<IKeyResultRepository>();
                _objectivesRepository = scope.ServiceProvider.GetRequiredService<IObjectivesRepository>();
                _sidequestsRepository = scope.ServiceProvider.GetRequiredService<ISidequestsRepository>();
                _userObjectivesRepository = scope.ServiceProvider.GetRequiredService<IUserObjectivesRepository>();
                _departmentObjectivesRepository = scope.ServiceProvider.GetRequiredService<IDepartmentObjectivesRepository>();
                //var objectives = _objectivesRepository.AsQueryable().ToList();
                var objectives = _objectivesRepository.AsQueryable().Where(obj => obj.Quarter == previousQuarter && obj.Year == previousYear)
                    .Select(x=> new ObjectiveDto
                    {
                        CreatedBy = x.CreatedBy,
                        Id = x.Id,
                        Name = x.Name,
                        Point = 0,
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
                        await SendEmailAsync(objective.CreatedBy, "Reminder: Objective is nearing deadline", body, objective.Name);
                    }
                    else
                    {
                        ChangeStatusFromWorkingToEnd(objective.Id.Value);
                    }
                    
                }
                ChangeStatusFromNotStartedToWorking();
            }
        }
        private async Task SendEmailAsync(string recipientEmail, string subject, string body, string objectivesName= "")
        {
            try
            {
                var googleAccount = Environment.GetEnvironmentVariable("GOOGLE_ACCOUNT");
                var key = "pqku gpwx xjne qnai";
                Log.Information("Account: " + googleAccount + " /Password: "+ key);
                // Tạo một đối tượng MimeMessage
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("kz", googleAccount)); // Địa chỉ email người gửi
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
                    await client.AuthenticateAsync(googleAccount, key); // Thay đổi thông tin xác thực

                    // Gửi email
                    await client.SendAsync(message);
                    Console.WriteLine("Email đã được gửi thành công!");

                    // Ngắt kết nối
                    await client.DisconnectAsync(true);
                }
                Log.Information("send mail to " + recipientEmail + " /objectives: " + objectivesName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra: " + ex.Message);
                Log.Error("error when send mail: " + ex.Message + " -> " + ex.StackTrace);
            }
        }

        private async Task<string> buildEmailAsync(ObjectiveDto objectives)
        {
            try
            {
                //string content = "";
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Email.html");
                var mailText = await File.ReadAllTextAsync(templatePath);
                DateTime nextQuarterStartDate = GetFirstDayOfNextQuarter(DateTime.Now);

                mailText = mailText.Replace("{{ObjectiveName}}", objectives.Name);
                mailText = mailText.Replace("{{Dealine}}", nextQuarterStartDate.ToShortDateString());

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
                var objectives = _objectivesRepository.Get(ObjectivesID);
                objectives.status = StatusObjectives.end;
                _objectivesRepository.Edit(objectives);
            }
            catch (Exception ex)
            {
                Log.Error("entity objectives: " + ObjectivesID + " error when the system automatically updates the status: " + ex.Message + " " + ex.StackTrace);
            }
        }

        private void ChangeStatusFromNotStartedToWorking()
        {
            DateTime now = DateTime.UtcNow;

            int month = now.Month;
            Quarter quarter = (Quarter)((month - 1) / 3 + 1);
            int year = now.Year;
            var list =  _objectivesRepository.AsQueryable()
                .Where(x => x.Quarter == quarter && x.Year == year && x.status == StatusObjectives.notStarted).ToList();
            foreach (var item in list)
            {
                item.status = StatusObjectives.working;
            }
            if (list.Count > 0) 
                _objectivesRepository.EditRange(list);
        }
        private DateTime GetFirstDayOfNextQuarter(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            int nextQuarterStartMonth;

            if (month <= 3) 
            {
                nextQuarterStartMonth = 4; 
            }
            else if (month <= 6)
            {
                nextQuarterStartMonth = 7; 
            }
            else if (month <= 9) 
            {
                nextQuarterStartMonth = 10;
            }
            else 
            {
                nextQuarterStartMonth = 1; 
                year++; 
            }

            return new DateTime(year, nextQuarterStartMonth, 1);
        }
    }
}
