using System.ComponentModel.DataAnnotations.Schema;

namespace AxelCMS.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        [ForeignKey("ProductId")]
        public string ProductId { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Quantity { get; set; }
    }
}
