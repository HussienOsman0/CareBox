using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class AppUser : IdentityUser<int>
    {
        public virtual Client Client { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


        public string? OTPCode { get; set; } 
        public DateTime? OTPExpiryTime { get; set; }
    }
}
