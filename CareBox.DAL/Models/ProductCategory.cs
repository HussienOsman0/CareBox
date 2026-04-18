using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // مربوطة بالـ Provider لضمان الخصوصية

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
