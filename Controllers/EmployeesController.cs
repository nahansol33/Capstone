using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;
using Capstone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Capstone.Controllers
{
	[Authorize(Roles = "Admin,Manager")]
	public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;

		public EmployeesController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_roleManager = roleManager;
			_userManager = userManager;
		}

		// GET: Employees
		public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

			var roles = _roleManager.Roles.Select(r => new SelectListItem
			{
				Value = r.Name,
				Text = r.Name
			}).ToList();

			// Pass the roles to the view using ViewData
			ViewData["Roles"] = roles;

			return View(employee);
        }

		// GET: Employee/Create
		public IActionResult Create()
		{
			var roles = new List<SelectListItem>
			{
				new SelectListItem { Value = "Admin", Text = "Admin" },
				new SelectListItem { Value = "Manager", Text = "Manager" },
				new SelectListItem { Value = "Employee", Text = "Employee" }
			};

			ViewData["Roles"] = roles;

			return View();
		}

		// POST: Employees/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("EmployeeId, Name, Email, Role")] Employee employee)
		{
			if (ModelState.IsValid)
			{
				// Create a new IdentityUser and assign a password
				var identityUser = new IdentityUser
				{
					UserName = employee.Email,
					Email = employee.Email
				};

				// Create the IdentityUser in the database
				var result = await _userManager.CreateAsync(identityUser, "Test123!");

				if (result.Succeeded)
				{
					// Assign the role to the new employee based on the role selected during creation
					if (employee.Role != null)
					{
						await _userManager.AddToRoleAsync(identityUser, employee.Role);
					}

					// Add the employee to the database
					_context.Add(employee);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				else
				{
					// If there are any errors during user creation, add them to the model state
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
				}
			}

			return View(employee);
		}
		// GET: Employees/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }


            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Role")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
