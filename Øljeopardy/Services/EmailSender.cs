using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oljeopardy.Models;
using Oljeopardy.Models.Email;

namespace Oljeopardy.Services
{

    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;

        public EmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var emailMessage = new EmailMessage();
                emailMessage.ToAddresses.Add(new EmailAddress(){Address = email});
                emailMessage.FromAddresses.Add(new EmailAddress(){Address = "noreply@copalex.com"});
                emailMessage.Subject = subject;
                emailMessage.Content = message;
                _emailService.Send(emailMessage);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
               throw new Exception("Could not send email");
            }
        }
    }
}
