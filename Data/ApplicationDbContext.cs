using Capstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Data
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public DbSet<Employee> Employees { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<TaskItem> TaskItems { get; set; }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}
}
