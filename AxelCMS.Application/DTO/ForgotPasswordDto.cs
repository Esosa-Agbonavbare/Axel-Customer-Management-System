using System.ComponentModel.DataAnnotations;

namespace AxelCMS.Application.DTO
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
