using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class ShowGames : PageModel
{
    public string Username { get; set; } = default!;
    public IActionResult OnGet(string username)
    {
        if (TempData.ContainsKey("Username"))
        {
            Username = TempData["Username"] as string;
            return Page();
        }
        else
        {
             return RedirectToPage("Index");
        }
    }
}