using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxelCMS.Application.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        public string Password { get; set; }
    }
}
