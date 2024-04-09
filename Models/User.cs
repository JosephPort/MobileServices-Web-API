using System.ComponentModel.DataAnnotations;

namespace MobileServices_Web_API.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
    }
}
