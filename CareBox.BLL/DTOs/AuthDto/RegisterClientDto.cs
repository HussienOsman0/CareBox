using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.AuthDto
{
    public class RegisterClientDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "The password must be at least 8 letters or numbers long.")]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

    }
}
