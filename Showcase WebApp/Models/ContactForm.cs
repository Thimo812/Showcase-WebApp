using Showcase_WebApp.Exceptions;
using System.Text.RegularExpressions;

namespace Showcase_WebApp.Models
{
    public class ContactForm
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
