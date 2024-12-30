using Microsoft.EntityFrameworkCore;
using Kanban_board.Data;
using Microsoft.AspNetCore.Identity;
using Kanban_board.Areas.Identity.Data;

namespace Kanban_board
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("KanbanContextConnection")
                                  ?? throw new InvalidOperationException("Connection string 'KanbanContextConnection' not found.");

            builder.Services.AddDbContext<Kanban_boardContext>(options =>
                          options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 6; 
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequireDigit = true; 
                options.Password.RequireUppercase = true; 
            })
           .AddRoles<IdentityRole>() // Szerepkörök regisztrálása
           .AddEntityFrameworkStores<Kanban_boardContext>();


            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await DataSeeder.SeedRoles(roleManager);
                await DataSeeder.SeedAdmin(userManager, roleManager); 
                await DataSeeder.SeedUser(userManager, roleManager);  
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapGet("/", async context =>
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Response.Redirect("/Boards/Index"); 
                }
                else
                {
                    context.Response.Redirect("Identity/Account/Login"); // Bejelentkezési oldal
                }
                await context.Response.CompleteAsync();
            });
            app.MapRazorPages();

            app.Run();
        }
    }
}
