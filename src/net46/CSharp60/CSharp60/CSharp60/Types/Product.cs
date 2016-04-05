using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp60.Types
{
    // Auto Property Initialization and string interpolation
    public class Product
    {
        public string ProductName { get; set; }

        public Guid ProductId { get; set; } = Guid.NewGuid();

        public Guid ParentProductCategoryId { get; } = Guid.NewGuid();

        public String DisplayProduct() => $"{this.ProductId} { this.ParentProductCategoryId }";

        public String ProductInfo => $"{this.ProductId} { this.ParentProductCategoryId }";

    }
}
