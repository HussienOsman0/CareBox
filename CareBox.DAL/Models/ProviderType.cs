using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class ProviderType
    {
        public byte ProviderTypeId { get; set; } 
        public string TypeName { get; set; }

        public virtual ICollection<ServiceProvider> ServiceProviders { get; set; }
    }
}
