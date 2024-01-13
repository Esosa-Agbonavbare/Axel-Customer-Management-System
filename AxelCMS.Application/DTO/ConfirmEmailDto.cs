using System.ComponentModel.DataAnnotations;

namespace AxelCMS.Application.DTO
{
    public class ConfirmEmailDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
