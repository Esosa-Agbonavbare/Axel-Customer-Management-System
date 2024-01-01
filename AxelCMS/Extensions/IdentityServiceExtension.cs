using AxelCMS.Domain.Entities;
using AxelCMS.Persistence.Context;
using Microsoft.AspNetCore.Identity;

namespace AxelCMS.Extensions
{
    public static class IdentityServiceExtension
    {
        public static void IdentityConfiguration(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
            });
            builder.AddEntityFrameworkStores<AxelCMSDbContext>().AddDefaultTokenProviders();
        }
    }
}
