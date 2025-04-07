using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Capstone.Models.ViewModels
{
	public class ProjectCreateViewModel
	{
		public string ProjectName { get; set; }

		public List<int> SelectedEmployeeIds { get; set; } = new List<int>();

		[ValidateNever]
		public List<SelectListItem> AvailableEmployees { get; set; }

		public List<TaskItemViewModel> Tasks { get; set; } = new List<TaskItemViewModel>();
	}

	public class TaskItemViewModel
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Status { get; set; }
		public int ProjectId { get; set; }
	}

}
