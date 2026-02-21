using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ClientDto.Profile
{
    public class ClientProfileDto
    {
        public string FullName { get; set; }
        public string Email { get; set; } // جاي من AppUser
        public string PhoneNumber { get; set; } // جاي من AppUser
        public string? Address { get; set; }
        public string? ClientImageUrl { get; set; }
    }
}
