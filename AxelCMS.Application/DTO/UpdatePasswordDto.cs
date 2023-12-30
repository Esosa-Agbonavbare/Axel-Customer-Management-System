using System.ComponentModel.DataAnnotations;

namespace AxelCMS.Application.DTO
{
    public class UpdatePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}