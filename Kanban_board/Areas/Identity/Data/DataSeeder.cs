using Microsoft.AspNetCore.Identity;

namespace Kanban_board.Areas.Identity.Data
{
    public class DataSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Admin szerep létrehozása
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // User szerep létrehozása
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        public static async Task SeedAdmin(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminEmail = "admin@example.com";
            var adminPassword = "Minda1"; // Jelszó

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = "admin", 
                    Email = adminEmail
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {

                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("Admin felhasználó létrehozva.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Hiba: {error.Description}");
                    }
                }
            }
            else
            {
     
                var isPasswordValid = await userManager.CheckPasswordAsync(adminUser, adminPassword);
                Console.WriteLine($"Admin jelszó: {adminPassword}");
                if (!isPasswordValid)
                {
                    Console.WriteLine("Az admin jelszó érvénytelen.");
                }
                else
                {
                    Console.WriteLine("Az admin jelszó helyes.");
                }
            }
        }

        public static async Task SeedUser(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var userEmail = "user@example.com";
            var userPassword = "User12"; // Jelszó

            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = "user",
                    Email = userEmail
                };

                var result = await userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                {
                    // Szerep hozzáadása
                    await userManager.AddToRoleAsync(user, "User");
                    Console.WriteLine("User felhasználó létrehozva.");
                }
                else
                {
               
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Hiba: {error.Description}");
                    }
                }
            }
            else
            {
       
                var isPasswordValid = await userManager.CheckPasswordAsync(user, userPassword);
                Console.WriteLine($"User jelszó: {userPassword}");
                if (!isPasswordValid)
                {
           
                    Console.WriteLine("A felhasználó jelszó érvénytelen.");
                }
                else
                {
                    Console.WriteLine("A felhasználó jelszó helyes.");
                }
            }
        }
    }
}