using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
