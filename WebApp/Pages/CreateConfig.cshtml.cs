using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class CreateConfig : PageModel
{
    private readonly IConfigRepository _configRepository;
    
    public string Message { get; set; } = String.Empty;
    public GameConfig AddConfig { get; set; } = default!;
    public CreateConfig(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public IActionResult OnGet(string message)
    {
        Message = message;
        AddConfig = new GameConfig();
        return Page();
    }
    
    [BindProperty]
    public string ConfigurationName { get; set; } = string.Empty;
    [BindProperty]
    public int BoardHeight { get; set; }
    [BindProperty]
    public int BoardWidth { get; set; }
    [BindProperty]
    public int GridSizeWinCondition { get; set; }
    [BindProperty]
    public int XCoords { get; set; }
    [BindProperty]
    public int YCoords { get; set; }
    [BindProperty]
    public int NrOfGamePieces { get; set; }
    [BindProperty]
    public int MovePiecesAfterNMoves { get; set; }

    public IActionResult OnPostCreate()
    {
        if (_configRepository.DoesConfigExist(ConfigurationName))
        {
            return OnGet("Game configuration with this name already exists");
        }
        AddConfig = new GameConfig();
        AddConfig.Name = ConfigurationName;
        AddConfig.BoardHeight = BoardHeight;
        AddConfig.BoardWidth = BoardWidth;
        AddConfig.GridSizeAndWinCondition = GridSizeWinCondition;
        AddConfig.GridStartPosX = XCoords;
        AddConfig.GridStartPosY = YCoords;
        AddConfig.GamePiecesPerPlayer = NrOfGamePieces;
        AddConfig.GamePiecesPerPlayer = MovePiecesAfterNMoves;
        _configRepository.CreateGameConfig(AddConfig);
        
        return RedirectToPage("ShowGames", new { message = "Configuration created successfully" });
    }
}