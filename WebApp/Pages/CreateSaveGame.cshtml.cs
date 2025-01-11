using System.Drawing;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class CreateSaveGame : PageModel
{
    private readonly IGameRepository _gameRepository;
    private readonly IConfigRepository _configRepository;
    
    public List<string> ConfigurationNames { get; set; } = [];
    public string Username { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public CreateSaveGame(IGameRepository gameRepository, IConfigRepository configRepository)
    {
        _gameRepository = gameRepository;
        _configRepository = configRepository;
    }
    
    public IActionResult OnGet(string message)
    {
        Username = HttpContext.Session.GetString("Username")!;
        Message = message;
        ConfigurationNames = _configRepository.GetConfigurationNames();
        return Page();
    }

    
    [BindProperty]
    public string GameModeInput { get; set; } = string.Empty;
    [BindProperty]
    public string PlayerA { get; set; } = string.Empty;
    [BindProperty]
    public string PlayerB { get; set; } = string.Empty;
    [BindProperty]
    public string Configuration { get; set; } = string.Empty;
    public IActionResult OnPostCreate()
    {
        EGameMode gameMode = (EGameMode)int.Parse(GameModeInput);
        var chosenConfig = _configRepository.GetConfigurationByName(Configuration);
        var gameInstance = new TicTacTwoBrain(chosenConfig, gameMode, PlayerA, PlayerB);
        gameInstance.MoveTheGrid(new Point(chosenConfig.GridStartPosX, chosenConfig.GridStartPosY));
        gameInstance.saveGameName = _gameRepository.GenerateSaveGameName(PlayerA, PlayerB, gameMode, Configuration);
        _gameRepository.SaveGame(gameInstance.saveGameName, gameInstance._gameState.ToJsonString(), Configuration, PlayerA, PlayerB, gameMode);
        
        HttpContext.Session.SetString("SaveGameName", gameInstance.saveGameName);
        return RedirectToPage("GameBoard", new { message = "Game created successfully" });
    }
}