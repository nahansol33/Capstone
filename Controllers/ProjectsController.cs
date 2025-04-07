using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;
using Capstone.Models;
using Capstone.Models.ViewModels;

namespace Capstone.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }

		// GET: Projects/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var project = await _context.Projects
				.Include(p => p.AssignedEmployees) 
				.FirstOrDefaultAsync(m => m.ProjectId == id);

			if (project == null)
			{
				return NotFound();
			}

			return View(project);
		}


		// GET: Projects/Create
		public IActionResult Create()
		{
			var viewModel = new ProjectCreateViewModel
			{
				// Populate the AvailableEmployees list
				AvailableEmployees = _context.Employees
		   .Where(e => e.ProjectId == null) // Assuming you want employees not assigned to a project
		   .Select(e => new SelectListItem
		   {
			   Value = e.EmployeeId.ToString(),
			   Text = e.Name
		   }).ToList()
			};

			return View(viewModel);
		}


		// POST: Projects/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProjectCreateViewModel viewModel)
		{
			// Debugging the submitted form data
			Console.WriteLine($"ProjectName: {viewModel.ProjectName}");
			Console.WriteLine($"Selected Employee IDs: {string.Join(", ", viewModel.SelectedEmployeeIds)}");
			Console.WriteLine($"Number of Tasks: {viewModel.Tasks.Count}");

			if (ModelState.IsValid)
			{
				var project = new Project
				{
					ProjectName = viewModel.ProjectName,
					Tasks = viewModel.Tasks.Select(t => new TaskItem
					{
						Title = t.Title,
						Description = t.Description,
						Status = t.Status
					}).ToList()
				};

				_context.Projects.Add(project);
				await _context.SaveChangesAsync();

				// Assign employees to the newly created project
				var employees = _context.Employees
					.Where(e => viewModel.SelectedEmployeeIds.Contains(e.EmployeeId));

				foreach (var emp in employees)
				{
					emp.ProjectId = project.ProjectId;
				}

				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}

			// Repopulate AvailableEmployees in case of validation failure
			viewModel.AvailableEmployees = _context.Employees
				.Where(e => e.ProjectId == null)
				.Select(e => new SelectListItem
				{
					Value = e.EmployeeId.ToString(),
					Text = e.Name
				}).ToList();

			return View(viewModel);
		}




		// GET: Projects/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var project = await _context.Projects
				.Include(p => p.AssignedEmployees)  // Include employees related to the project
				.FirstOrDefaultAsync(p => p.ProjectId == id);

			if (project == null)
			{
				return NotFound();
			}

			// Create the view model to pass data to the view
			var viewModel = new ProjectCreateViewModel
			{
				ProjectName = project.ProjectName,
				// Pass the selected employees' IDs into the ViewModel
				SelectedEmployeeIds = project.AssignedEmployees.Select(e => e.EmployeeId).ToList(),
				AvailableEmployees = _context.Employees
					.Where(e => e.ProjectId == null || e.ProjectId == project.ProjectId) // Employees either unassigned or assigned to this project
					.Select(e => new SelectListItem
					{
						Value = e.EmployeeId.ToString(),
						Text = e.Name
					}).ToList()
			};

			// Add ProjectId to ViewData for later use in the POST action
			ViewData["ProjectId"] = project.ProjectId;

			return View(viewModel);
		}


		// POST: Projects/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, ProjectCreateViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				// Fetch the project including its employees
				var project = await _context.Projects
					.Include(p => p.AssignedEmployees)
					.FirstOrDefaultAsync(p => p.ProjectId == id);

				if (project == null)
				{
					return NotFound();
				}

				// Update project name
				project.ProjectName = viewModel.ProjectName;

				// Remove employees who were unselected (unchecked)
				var employeesToRemove = project.AssignedEmployees
					.Where(e => !viewModel.SelectedEmployeeIds.Contains(e.EmployeeId))
					.ToList();

				foreach (var employee in employeesToRemove)
				{
					employee.ProjectId = null;  // Unassign employee from project
				}

				// Add employees who were selected but were not already assigned
				var employeesToAdd = _context.Employees
					.Where(e => viewModel.SelectedEmployeeIds.Contains(e.EmployeeId) && e.ProjectId == null)
					.ToList();

				foreach (var employee in employeesToAdd)
				{
					employee.ProjectId = id;  // Assign employee to project
				}

				// Save changes to the database
				await _context.SaveChangesAsync();

				// Redirect to the index or project details page
				return RedirectToAction(nameof(Index));
			}

			// Repopulate AvailableEmployees if validation fails
			viewModel.AvailableEmployees = _context.Employees
				.Where(e => e.ProjectId == null || e.ProjectId == id)
				.Select(e => new SelectListItem
				{
					Value = e.EmployeeId.ToString(),
					Text = e.Name
				}).ToList();

			return View(viewModel);
		}




		// GET: Projects/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

		// POST: Projects/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var project = await _context.Projects
				.Include(p => p.AssignedEmployees) // include related employees
				.FirstOrDefaultAsync(m => m.ProjectId == id);

			if (project == null)
			{
				return NotFound();
			}

			if (project.AssignedEmployees.Any())
			{
				ModelState.AddModelError("", "Cannot delete project because it has assigned employees or tasks.");
				return View(project); // or redirect with TempData error
			}

			_context.Projects.Remove(project);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

	}
}
