using System.ComponentModel.DataAnnotations.Schema;

namespace AxelCMS.Domain.Entities
{
    public class Order : BaseEntity
    {
        public DateTimeOffset OrderDate => CreatedAt;
        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        [ForeignKey("UserId")]
        public string UserId { get; set; }
        [ForeignKey("ShippingAddressId")]
        public string ShippingAddressId { get; set; }
    }
}
