using AxelCMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace AxelCMS.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<Address> AddressList { get; set; } = new List<Address>();
        public UserRole Role { get; set; }
        public Gender Gender { get; set; }
        public string ImageUrl { get; set; }
        public string VerificationToken { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime ResetTokenExpires { get; set; }
    }
}
