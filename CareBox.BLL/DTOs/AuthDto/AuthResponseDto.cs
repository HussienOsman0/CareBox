using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.AuthDto
{
    public class AuthResponseDto
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        //  Access Token (JWT)
        public string Token { get; set; }
        public DateTime? TokenExpiration { get; set; } 

        // Refresh Token
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

        public List<string> Errors { get; set; }
    }
}
