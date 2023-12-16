using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxelCMS.Application.Interfaces.Services
{
    public interface ICloudinaryService<T> where T : class
    {
        Task<string> UploadImage(string entityId, IFormFile file);
    }
}
