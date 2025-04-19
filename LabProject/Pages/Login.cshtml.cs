using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using ClassManApp.Models;

namespace LabProject.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        var jsonPath = Path.Combine("wwwroot", "data", "users.json");
        if (!System.IO.File.Exists(jsonPath))
        {
            ErrorMessage = "User data not found.";
            return Page();
        }

        var json = System.IO.File.ReadAllText(jsonPath);
        var users = JsonSerializer.Deserialize<List<User>>(json);

        var matched = users.FirstOrDefault(u =>
            u.Username == Username &&
            u.Password == Password &&
            u.IsActive);

        if (matched == null)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        // başarıyla giriş
        HttpContext.Session.SetString("username", matched.Username);
        HttpContext.Session.SetString("token", Guid.NewGuid().ToString());
        HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

        return RedirectToPage("/ClassManApp");
    }
}
