using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Capstone.Pages.Account
{
	public class RegisterModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public class InputModel
		{
			[Required]
			[EmailAddress]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }

			[Required]
			public string Role { get; set; }
		}

		[BindProperty]
		public InputModel Input { get; set; }

		// Role list to populate dropdown
		public List<SelectListItem> RoleList { get; set; }

		public string ReturnUrl { get; set; }

		public async Task OnGetAsync(string returnUrl = null)
		{
			RoleList = new List<SelectListItem>
			{
				new SelectListItem { Value = "Admin", Text = "Admin" },
				new SelectListItem { Value = "Manager", Text = "Manager" },
				new SelectListItem { Value = "Employee", Text = "Employee" }
			};

			ReturnUrl = returnUrl ?? Url.Content("~/");
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			RoleList = new List<SelectListItem>
			{
				new SelectListItem { Value = "Admin", Text = "Admin" },
				new SelectListItem { Value = "Manager", Text = "Manager" },
				new SelectListItem { Value = "Employee", Text = "Employee" }
			};

			if (ModelState.IsValid)
			{
				var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
				var result = await _userManager.CreateAsync(user, Input.Password);

				if (result.Succeeded)
				{
					var assignedRole = string.IsNullOrEmpty(Input.Role) ? "Employee" : Input.Role;

					await _userManager.AddToRoleAsync(user, assignedRole);
					await _signInManager.SignInAsync(user, isPersistent: false);

					return LocalRedirect(returnUrl);
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return Page();
		}
	}
}
