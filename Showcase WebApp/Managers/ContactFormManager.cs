using SendGrid;
using SendGrid.Helpers.Mail;
using Showcase_WebApp.Models;
using System.Diagnostics;
using System.Net;

namespace Showcase_WebApp.Managers
{
    public class ContactFormManager
    {
        private IConfiguration _config;

        public ContactFormManager()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        }

        public async Task<int> SendMail(ContactForm form)
        {
            var apiKey = _config["SendMailKey"];

            var targetMail = _config.GetSection("ContactTargetInfo")["Email"];
            var targetName = _config.GetSection("ContactTargetInfo")["Name"];

            var message = new SendGridMessage
            {
                From = new EmailAddress("showcasemailservice@gmail.com", form.FirstName),
                Subject = "Nieuw contactverzoek",
                PlainTextContent = $"{form.FirstName} {form.LastName} wilt contact met u opnemen",
                HtmlContent = $"<strong>{form.FirstName} wilt contact met u opnemen</strong>" +
                              $"<p>Naam: {form.FirstName} {form.LastName} </p>" +
                              $"<p>Email: {form.Email}</p>" +
                              $"<p>Telefoonnummer: {form.Phone}</p>"
            };

            message.AddTo(new EmailAddress(targetMail, targetName));

            var client = new SendGridClient(apiKey);

            var response = await client.SendEmailAsync(message);

            return (int) response.StatusCode;
        }

    }
}
