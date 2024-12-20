﻿using Microsoft.EntityFrameworkCore;
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
            var now = DateTime.UtcNow;
            var list =  _objectivesRepository.AsQueryable()
                .Where(x => x.StartDay >= now && x.status == StatusObjectives.notStarted).ToList();
            foreach (var item in list)
            {
                item.status = StatusObjectives.working;
            }

            _objectivesRepository.EditRange(list);
        }
    }
}
