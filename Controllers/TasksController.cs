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
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TaskItems.Include(t => t.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

		// GET: Tasks/Create
		public IActionResult Create()
		{
			// Populate the Project dropdown
			ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName");
			return View();
		}

		// POST: Tasks/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(TaskItemViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				// Create the TaskItem object and assign properties
				var taskItem = new TaskItem
				{
					Title = viewModel.Title,
					Description = viewModel.Description,
					Status = viewModel.Status,
					ProjectId = Convert.ToInt32(Request.Form["ProjectId"])  // Get the ProjectId from the form
				};

				// Add the task to the context
				_context.Add(taskItem);

				// Save changes to the database
				await _context.SaveChangesAsync();

				// Redirect to the Index page after successful creation
				return RedirectToAction(nameof(Index));
			}

			// If validation fails, repopulate the Project dropdown
			ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName");
			return View(viewModel);
		}



		// GET: Tasks/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var taskItem = await _context.TaskItems.FindAsync(id);
			if (taskItem == null)
			{
				return NotFound();
			}

			// Manually map TaskItem to TaskItemViewModel
			var viewModel = new TaskItemViewModel
			{
				Title = taskItem.Title,
				Description = taskItem.Description,
				Status = taskItem.Status,
				ProjectId = taskItem.ProjectId // Add this property in your ViewModel if it isn't there yet
			};

			ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", taskItem.ProjectId);
			return View(viewModel); // Send ViewModel to the view
		}


		// POST: Tasks/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, TaskItemViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				// Find the task by id
				var task = await _context.TaskItems.FindAsync(id);
				if (task == null)
				{
					return NotFound();
				}

				// Update task details from the view model
				task.Title = viewModel.Title;
				task.Description = viewModel.Description;
				task.Status = viewModel.Status;  // Update the status
				task.Project = _context.Projects.FirstOrDefault(p => p.ProjectId == viewModel.ProjectId);

				// Save the changes
				await _context.SaveChangesAsync();

				// Redirect to the Index page or another page
				return RedirectToAction(nameof(Index));
			}

			// If validation fails, repopulate the Project dropdown and pass the view model back to the view
			ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectName", viewModel.ProjectId);
			return View(viewModel);
		}



		// GET: Tasks/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem != null)
            {
                _context.TaskItems.Remove(taskItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.TaskId == id);
        }
    }
}
