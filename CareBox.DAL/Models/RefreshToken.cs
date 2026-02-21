using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string TokenHash { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string DeviceId { get; set; }

        public bool IsRevoked { get; set; }

        // هنا بنغير UserId لي int لأنه مطابق ل PK في IdentityUser<int>
        public int UserId { get; set; }

        // Navigation property
        public AppUser User { get; set; }
    }
}
