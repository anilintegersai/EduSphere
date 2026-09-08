using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Account;

public class ActivateModel : PageModel
{
    private readonly IUserManagementService _users;

    public ActivateModel(IUserManagementService users)
    {
        _users = users;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? StatusMessage { get; private set; }
    public bool IsComplete { get; private set; }

    public class InputModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public IActionResult OnGet(Guid userId, string? code)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(code))
        {
            StatusMessage = "Activation link is invalid.";
            return Page();
        }

        Input.UserId = userId;
        Input.Code = code;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _users.ActivateAccountAsync(Input.UserId, Input.Code, Input.NewPassword, HttpContext.RequestAborted);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        IsComplete = true;
        StatusMessage = result.Message;
        return Page();
    }
}
