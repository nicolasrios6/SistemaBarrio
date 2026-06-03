using Microsoft.AspNetCore.Identity;
using SistemaBarrio.Models;

namespace SistemaBarrio.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<Usuario>>();

            string[] roles = { "Admin", "Guardia" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string email = "nicolasrios.dev@gmail.com";
            string password = "Admin123!";

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                var user = new Usuario
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            string guardiaEmail = "guardia@barriocarolinos.com";
            string guardiaPassword = "Guardia123!";

            var guardia = await userManager.FindByEmailAsync(guardiaEmail);
            if (guardia == null)
            {
                var user = new Usuario
                {
                    UserName = guardiaEmail,
                    Email = guardiaEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, guardiaPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Guardia");
            }
        }
    }
}
