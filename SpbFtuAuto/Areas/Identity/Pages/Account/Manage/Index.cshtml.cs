using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpbFtuAuto.Data;

namespace SpbFtuAuto.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email в системе SpbFtu")]
            public string SpbFtuEmail { get; set; }
            [Required]
            [Display(Name = "Пароль в системе SpbFtu")]
            public string SpbFtuPassword { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            User User = await _userManager.FindByNameAsync(userName);
            var SpbFtuEmail = User.FtuEmail;
            var SpbFtuPassword = User.FtuPassword;

            Username = userName;

            Input = new InputModel
            {
                SpbFtuEmail = SpbFtuEmail,
                SpbFtuPassword = SpbFtuPassword
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var SpbFtuEmail = user.FtuEmail;
            if (Input.SpbFtuEmail != SpbFtuEmail)
            {
                var setPhoneResult = user.FtuEmail = SpbFtuEmail;
                _ = _userManager.UpdateAsync(user);
            }

            var SpbFtuPassword = user.FtuPassword;
            if (Input.SpbFtuPassword != SpbFtuPassword)
            {
                var setPhoneResult = user.FtuEmail = SpbFtuEmail;
            }
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
