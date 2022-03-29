using Hire360WebAPI.Models;

namespace Hire360WebAPI.Services
{
    public interface IMailService
    {
        public Task SendEmailAsync(string ToEmail, string Subject, string Body);
        public Task SendWelcomeEmailAsync(string Candidateemail, string name);
    }
}