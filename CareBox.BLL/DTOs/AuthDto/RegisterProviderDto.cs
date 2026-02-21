using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.AuthDto
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RegisterProviderDto
    {
        // --- Identity Info ---
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        // --- Provider Profile Info ---
        [Required]
        public string Name { get; set; } // Business Name

        [Required]
        [Range(1, 4, ErrorMessage = "ProviderTypeId must be between 1 and 4.")]
        public byte ProviderTypeId { get; set; } // نوع الخدمة (ميكانيكي، كهربائي...)

        [Required]
        public string Address { get; set; }

        // Working Hours as text in "HH:mm - HH:mm" format
        [Required]
        [RegularExpression(@"^([0-1]\d|2[0-3]):([0-5]\d) - ([0-1]\d|2[0-3]):([0-5]\d)$",
            ErrorMessage = "WorkingHours must be in the format HH:mm - HH:mm")]
        public string WorkingHours { get; set; }

        // Geolocation (Front-end sends numbers)
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public IFormFile? LogoImage { get; set; }
    }

}
