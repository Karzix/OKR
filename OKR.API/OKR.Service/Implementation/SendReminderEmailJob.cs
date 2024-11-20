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
                //_sidequestsRepository = scope.ServiceProvider.GetRequiredService<ISidequestsRepository>();
                //var objectives = _objectivesRepository.AsQueryable().ToList();
                var objectives = _objectivesRepository.FindByPredicate(x => x.EndDay.Date <= thresholdDate.Date && x.EndDay.Date >= currentDate.Date)
                    .Select(x => new ObjectivesRespone
                    {
                        CreatedBy = x.CreatedBy,
                        Id = x.Id,
                        Name = x.Name,
                        Point = 0,
                        TargetType = x.TargetType,
                        KeyResults = _keyResultRepository.AsQueryable().Where(k => k.ObjectivesId == x.Id).Select(k => new KeyResultRespone
                        {
                            CurrentPoint = k.CurrentPoint,
                            EndDay = k.Deadline,
                            Description = k.Description,
                            Id = k.Id,
                            MaximunPoint = k.MaximunPoint,
                            Note = "",
                            Unit = k.Unit,
                            Status = k.Status,
                        }).ToList(),
                        EndDay = x.EndDay,
                        
                    })
                    .ToList();
                var dateNow = DateTime.UtcNow;
                foreach (var objective in objectives)
                {
                    if (objective.EndDay.Value != dateNow)
                    {
                        var body = await buildEmailAsync(objective);
                        await SendEmailAsync(objective.CreatedBy, "Reminder: Objective is nearing deadline", body, objective.Name);
                    }
                    else
                    {
                    }

                }
            }
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string body, string objectivesName= "")
        {
            try
            {
                var googleAccount = "nakiet.kn@gmail.com";
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

        private async Task<string> buildEmailAsync(ObjectivesRespone objectives)
        {
            try
            {
                //string content = "";
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Email.html");
                //var templatePath = @"D:\GitHub\OKRs\OKR.API\OKR.Service\Template\Email.html";
                var mailText = await File.ReadAllTextAsync(templatePath);
                DateTime nextQuarterStartDate = GetFirstDayOfNextQuarter(DateTime.Now);

                mailText = mailText.Replace("{{ObjectiveName}}", objectives.Name);
                mailText = mailText.Replace("{{Dealine}}", objectives.EndDay.Value.ToString("dd/MM/yyyy"));

                var listKeyresuls = _keyResultRepository.AsQueryable().Where(x => x.ObjectivesId == objectives.Id).ToList();
                var keyresults = BuildKeyresult(objectives.KeyResults);
                mailText = mailText.Replace("{{ListKeyresult}}", keyresults);

                mailText = mailText.Replace("{{Link}}", "http://103.209.34.217/Objectives=" + objectives.Id);
                return mailText;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ex.Message;
            }
        }

        private string BuildKeyresult(List<KeyResultRespone> keyResultDto)
        {
            string content = "";
            foreach (var keyResult in keyResultDto)
            {
                content += "<tr>" +
                              $"<td style=\"border: 1px solid #dddddd; padding: 10px;\">{keyResult.Description}</td>" + 
                              $"<td style=\"border: 1px solid #dddddd; padding: 10px; text-align: center;\">{keyResult.CurrentPoint}/{keyResult.MaximunPoint}</td>" +
                          "</tr>";
            }
            return content;
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
