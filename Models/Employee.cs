using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
	public class Employee
	{
		[Key]
		public int EmployeeId { get; set; }

		[Required]
		[StringLength(20)]
		public string Name { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		public string Role { get; set; }

		//adding ref to project 
		public int? ProjectId { get; set; }
		[ForeignKey("ProjectId")]
		public virtual Project? Project { get; set; }
	}
}
