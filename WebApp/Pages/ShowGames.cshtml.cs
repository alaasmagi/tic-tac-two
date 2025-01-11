using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class ShowGames : PageModel
{
    private readonly IGameRepository _gameRepository;
    
    public string Username { get; set; } = string.Empty;
    public List<string> SaveGames { get; set; } = new List<string>();
    public string Message { get; set; } = string.Empty;
    
    public ShowGames(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    public IActionResult OnGet(string message)
    {
        Username = HttpContext.Session.GetString("Username")!;
        Message = message;
        if (string.IsNullOrEmpty(Username))
        {
            return RedirectToPage("Index");
        }
        SaveGames = _gameRepository.GetSaveGameNames(Username);
        return Page();
    }

    [BindProperty]
    public string DeleteGameName { get; set; } = string.Empty;
    public IActionResult OnPostDelete()
    {
        _gameRepository.DeleteGame(DeleteGameName);
        return OnGet("Save game deleted successfully");
    }

    [BindProperty]
    public string SelectedGameName { get; set; } = string.Empty;
    public IActionResult OnPostSelect()
    {
        Username = HttpContext.Session.GetString("Username")!;
        HttpContext.Session.SetString("SaveGameName", SelectedGameName);
        return RedirectToPage("GameBoard", new { message = "Save game loaded successfully" });
    }
}

