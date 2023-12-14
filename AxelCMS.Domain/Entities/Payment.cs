using AxelCMS.Domain.Enums;

namespace AxelCMS.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
    }
}
