using AxelCMS.Domain.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AxelCMS.Application.DTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } 

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }
    }
}
