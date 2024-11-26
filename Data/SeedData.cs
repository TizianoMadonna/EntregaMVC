using Microsoft.AspNetCore.Identity;

namespace FinalMadonnaTizianoLab4.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Crear el rol "Admin" si no existe
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Empleado"))
            {
                await roleManager.CreateAsync(new IdentityRole("Empleado"));
            }

            var defaultRole = "Empleado";

            var mail = await userManager.FindByEmailAsync("tizianokuriger@gmail.com");
            if (mail == null) 
            {
                mail = new IdentityUser { UserName = "tizianokuriger@gmail.com", Email = "tizianokuriger@gmail.com" };
                await userManager.CreateAsync(mail, "Contra1");
                await userManager.AddToRoleAsync(mail, "Admin");
            }

            // Crear un usuario de prueba y asignarle el rol de Admin
            var userAdmin = await userManager.FindByEmailAsync("tizianomadonna334@gmail.com");
            if (userAdmin != null)
            {
                await userManager.AddToRoleAsync(userAdmin, "Admin");
            }
            var usuario1 = await userManager.FindByEmailAsync("cocacolasector1@gmail.com");
            var usuario2 = await userManager.FindByEmailAsync("cocacolasector2@gmail.com");
            var usuario3 = await userManager.FindByEmailAsync("cocacolasector3@gmail.com");
            if (usuario1 != null)
            {
                await userManager.AddToRoleAsync(usuario1, "Empleado");
            }
            if (usuario2 != null)
            {
                await userManager.AddToRoleAsync(usuario2, "Empleado");
            }
            if (usuario3 != null)
            {
                await userManager.AddToRoleAsync(usuario3, "Empleado");
            }
            //await userManager.CreateAsync(user, "Password123!");  // Cambia la contraseña en producción
            //await userManager.AddToRoleAsync(user, "Admin");
            if (userAdmin == null)
            {
                userAdmin = new IdentityUser { UserName = "tizianomadonna@gmail.com", Email = "tizianomadonna@gmail.com" };
                var result = await userManager.CreateAsync(userAdmin, "Tevoymatar3"); // Cambia la contraseña en producción

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(userAdmin, "Admin");
                    Console.WriteLine("Usuario admin creado y rol asignado.");
                }
                else
                {
                    Console.WriteLine($"Error al crear el usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Crear usuario Empleado y asignar rol
            var empleadoUser = await userManager.FindByEmailAsync("empleado@example.com");
            if (empleadoUser == null)
            {
                empleadoUser = new IdentityUser { UserName = "empleado@example.com", Email = "empleado@example.com" };
                await userManager.CreateAsync(empleadoUser, "Password123!");
                await userManager.AddToRoleAsync(empleadoUser, defaultRole);
            }
        }
    }
}
