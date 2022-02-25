using CityInformation.API.Interfaces;

namespace CityInformation.API.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = "admin@dev.com";
        private string _mailFrom = "noreply@dev.com";

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Cloud Mail from {_mailFrom} to {_mailTo}, with {nameof(CloudMailService)}.");
            Console.WriteLine($"Subject : {subject}");
            Console.WriteLine($"Message : {message}");
        }
    }
}
