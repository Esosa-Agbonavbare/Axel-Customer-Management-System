using AxelCMS.Application.Interfaces.Repositories;
using AxelCMS.Application.Interfaces.Services;
using AxelCMS.Domain.Entities;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AxelCMS.Application.ServicesImplementation
{
    public class CloudinaryService<TEntity> : ICloudinaryService<TEntity> where TEntity : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IGenericRepository<TEntity> repository, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            var account = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.APIKey,
                cloudinaryConfig.Value.APISecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImage(string entityId, IFormFile file)
        {
            var entity = await _repository.GetByIdAsync(entityId);

            if (entity == null)
            {
                return $"{typeof(User).Name} not found";
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                await _repository.UpdateAsync(entity);
                return uploadResult.SecureUrl.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "Image upload or database update error occurred";
            }            
        }
    }
}
