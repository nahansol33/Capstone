using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
	public class TaskItem
	{
		[Key]
		public int TaskId { get; set; }

		[Required]
		public string Title { get; set; }

		public string Description { get; set; }

		public string Status { get; set; }

		[ForeignKey("Project")]
		public int ProjectId { get; set; }

		public virtual Project Project { get; set; }
	}
}
