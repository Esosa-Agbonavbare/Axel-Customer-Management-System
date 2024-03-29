﻿using AxelCMS.Application.Interfaces.Repositories;
using AxelCMS.Application.Interfaces.Services;
using AxelCMS.Application.ServicesImplementation;
using AxelCMS.Domain.Entities;
using AxelCMS.Persistence.Context;
using AxelCMS.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AxelCMS.Extensions
{
    public static class DIServiceExtension
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var cloudinarySettings = configuration.GetSection("Cloudinary").Get<CloudinarySettings>();
            services.AddSingleton(cloudinarySettings);
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EmailSettings>>().Value);
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(ICloudinaryService<>), typeof(CloudinaryService<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddDbContext<AxelCMSDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
