using System.ComponentModel.DataAnnotations;

namespace AxelCMS.Application.DTO
{
    public class ValidateTokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}
