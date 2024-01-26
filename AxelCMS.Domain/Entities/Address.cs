using System.ComponentModel.DataAnnotations.Schema;

namespace AxelCMS.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string StreetLine1 { get; set; }
        public string StreetLine2 { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
    }
}
