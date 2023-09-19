using Microsoft.Extensions.Options;
using SendGrid;
using System.Threading.Tasks;
using AMS.Infrastructure.Email.Contracts;
using AMS.Infrastructure.Email.Models;
using System.Net.Mail;
using System.Net;
using System;

namespace AMS.Infrastructure.Email
{
    public class SendGridEmailProvider : IEmailProvider
    {
        private readonly EmailSettings _settings;
        private readonly ISendGridClient _client;

        public SendGridEmailProvider(IOptions<EmailSettings> emailSettings)
        {
            _settings = emailSettings.Value;
            _client = new SendGridClient(_settings.SendGrid.APIKey);
        }

        public async Task Send(SendRequest request)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(request.FromAddress);
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(new MailAddress(request.ToAddress));
            mailMessage.Subject = request.Subject;
            mailMessage.Body = request.Body;
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "sclopus18@gmail.com",  // replace with valid value
                    Password = "opus@123456" // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Timeout = 70000;
                //smtp.Send(mailMessage);

                //smtp.Port = 25;
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.UseDefaultCredentials = true;
                //smtp.EnableSsl = true;
                //smtp.Host = "103.15.246.58";
                try { await smtp.SendMailAsync(mailMessage); }
                catch (Exception e)
                {
                    throw;
                }

            }
            //var from = new EmailAddress(request.FromAddress);
            //var to = new EmailAddress(request.ToAddress);
            //var plainTextContent = Regex.Replace(request.Body, "<[^>]*>", "");
            //var msg = MailHelper.CreateSingleEmail(from, to, request.Subject, plainTextContent, request.Body);
            //var response = await _client.SendEmailAsync(msg);

            //if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            //{
            //    //todo: handle this better and log
            //    throw new System.Exception("Could not send email");
            //}
        }
    }
}
