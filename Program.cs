using Capstone.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone
{
	public class Program
	{
		public static async Task Main(string[] args) // Change to async Task
		{
			var builder = WebApplication.CreateBuilder(args);

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			builder.Services.AddDefaultIdentity<IdentityUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
			})
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>();

			builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();

            var app = builder.Build();

			// Seed the database on startup
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					await SeedData.Initialize(services);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error seeding database: {ex.Message}");
				}
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();	

            app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();
            app.MapControllers();

            app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			app.Run();
		}
	}
}

public static class SeedData
{
	public static async Task Initialize(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

		string[] roleNames = { "Admin", "Manager", "Employee" };

		foreach (var role in roleNames)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new IdentityRole(role));
			}
		}
		string adminEmail = "admin@example.com";
		string adminPassword = "Admin@123";

		var adminUser = await userManager.FindByEmailAsync(adminEmail);
		if (adminUser == null)
		{
			var newAdmin = new IdentityUser { UserName = adminEmail, Email = adminEmail };
			var result = await userManager.CreateAsync(newAdmin, adminPassword);
			if (result.Succeeded)
			{
				await userManager.AddToRoleAsync(newAdmin, "Admin");
			}
		}
	}
}