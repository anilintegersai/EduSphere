using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduSphere.Web.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly IUserManagementService _users;

    public ForgotPasswordModel(IUserManagementService users)
    {
        _users = users;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? StatusMessage { get; private set; }
    public IReadOnlyList<string> Warnings { get; private set; } = Array.Empty<string>();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _users.SendPasswordResetAsync(Input.Email, HttpContext.RequestAborted);
        StatusMessage = result.Message;
        Warnings = result.Warnings;
        return Page();
    }
}
