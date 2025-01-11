using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Username { get; set; } = default!;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(Username))
        { 
            return Page();
        }
        
        HttpContext.Session.SetString("Username", Username);
        return RedirectToPage("ShowGames", new { message = string.Empty });
    }
}
