using AxelCMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AxelCMS.Common.Utilities
{
    public class Seeder
    {
        public static void SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var seededAdmin = serviceProvider.GetRequiredService<UserManager<User>>();

            if (!roleManager.RoleExistsAsync("SuperAdmin").Result)
            {
                var role = new IdentityRole("SuperAdmin");
                roleManager.CreateAsync(role).Wait();
            }

            if (!roleManager.RoleExistsAsync("Manager").Result)
            {
                var role = new IdentityRole("Manager");
                roleManager.CreateAsync(role).Wait();
            }

            if (!roleManager.RoleExistsAsync("User").Result)
            {
                var role = new IdentityRole("User");
                roleManager.CreateAsync(role).Wait();
            }
        }
    }
}
