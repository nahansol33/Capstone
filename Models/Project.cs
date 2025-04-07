using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Capstone.Models
{
	public class Project
	{
		[Key]
		public int ProjectId { get; set; }

		[Required]
		public string ProjectName { get; set; }

		public virtual List<Employee>? AssignedEmployees { get; set; }
		public virtual List<TaskItem>? Tasks { get; set; }
	}
}
