﻿using System.ComponentModel.DataAnnotations;

namespace MobileServices_Web_API.Models
{
    public class UserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
