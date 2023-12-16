using Microsoft.AspNetCore.Http;

namespace AxelCMS.Application.DTO
{
    public class UpdatePhotoDto
    {
        public IFormFile PhotoFile { get; set; }
    }
}
