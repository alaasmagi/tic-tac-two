using System.Drawing;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class GameBoard : PageModel
{ 
    private readonly IGameRepository _gameRepository;
    
    public string Username { get; set; } = string.Empty;
    public string PlayerA { get; set; } = string.Empty;
    public string PlayerB { get; set; } = string.Empty;
    public GameState CurrentState { get; set; } = default!;
    public EGameMode GameMode { get; set; }
    public bool IsMovingGridOrPiecesUnlocked { get; set; }
    public string SaveGameName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public GameBoard(AppDbContext context, IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    public IActionResult OnGet(string message)
    {
        Message = message;
        Username = HttpContext.Session.GetString("Username")!;
        SaveGameName = HttpContext.Session.GetString("SaveGameName")!;
        
        _gameRepository.LoadGame(SaveGameName, out GameState state, out string playerA, out string playerB, out EGameMode gameMode);
        CurrentState = state;
        PlayerA = playerA;
        PlayerB = playerB;
        GameMode = gameMode;
        TicTacTwoBrain gameInstance = new TicTacTwoBrain(CurrentState.GameConfiguration, GameMode, PlayerA, PlayerB);
        gameInstance.SetGameState(CurrentState);
        gameInstance.saveGameName = SaveGameName;
        IsMovingGridOrPiecesUnlocked = gameInstance.IsGridOrExistingMoveUnlocked();

        if (!IsGameFinished(gameInstance))
        {
            CheckForAiMove(gameInstance);
        }
        
        return Page();
    }
    
    [BindProperty]
    public string GameButton { get; set; } = string.Empty;
    public IActionResult OnPostSelect()
    { 
        Username = HttpContext.Session.GetString("Username")!;
        SaveGameName = HttpContext.Session.GetString("SaveGameName")!;
        _gameRepository.LoadGame(SaveGameName, out GameState state, out string playerA, out string playerB, out EGameMode gameMode);
        
        TicTacTwoBrain gameInstance = new TicTacTwoBrain(state.GameConfiguration, gameMode, playerA, playerB);
        gameInstance._gameState = state;
        
        Point selectedButton = GetCoordinatesFromString(HttpContext.Session.GetString("ButtonToMove")!);
        Point nextMove = GetCoordinatesFromString(GameButton);
        bool isOperationSuccessful = false;
        
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("MoveGrid")))
        {
            isOperationSuccessful = gameInstance.MoveTheGrid(nextMove);
        }
        
        if (nextMove == new Point(-1, -1))
        {
            HttpContext.Session.SetString("ButtonToMove", string.Empty);
            return RedirectToPage("GameBoard", new { message = "Forbidden move" });
        }
        
        if (gameInstance._gameState.GameBoard[nextMove.X][nextMove.Y] ==
            gameInstance._gameState.NextMoveBy && selectedButton == new Point(-1, -1) && 
            gameInstance.IsGridOrExistingMoveUnlocked() && string.IsNullOrEmpty(HttpContext.Session.GetString("MoveGrid")))
        {
            HttpContext.Session.SetString("ButtonToMove", GameButton);
            return RedirectToPage("GameBoard", new { message = "Button selected, now select its new position" });
        }
        
        if (nextMove != selectedButton && selectedButton != new Point(-1, -1) && 
            string.IsNullOrEmpty(HttpContext.Session.GetString("MoveGrid")))
        {
            isOperationSuccessful = gameInstance.MoveExistingPiece(nextMove, selectedButton);
        }
        else if (string.IsNullOrEmpty(HttpContext.Session.GetString("MoveGrid")))
        {
            isOperationSuccessful = gameInstance.PlaceAPiece(nextMove);
        }

        HttpContext.Session.SetString("MoveGrid", string.Empty);
        HttpContext.Session.SetString("ButtonToMove", string.Empty);
        
        if (!isOperationSuccessful)
        {
            return RedirectToPage("GameBoard", new { message = "Forbidden move" });
        }
        
        _gameRepository.SaveGame(SaveGameName, gameInstance._gameState.ToJsonString(), 
            gameInstance._gameState.GameConfiguration.Name, playerA, playerB, gameMode);
        return RedirectToPage("GameBoard", new { message = string.Empty });
    }

    public void CheckForAiMove(TicTacTwoBrain gameInstance)
    {
        if ((gameInstance._gameMode == EGameMode.PlayerVsAi && gameInstance._gameState.NextMoveBy == EGamePiece.O) ||
            gameInstance._gameMode == EGameMode.AiVsAi)
        {
            AiBrain.AiMove(gameInstance);
            _gameRepository.SaveGame(SaveGameName, gameInstance._gameState.ToJsonString(),  gameInstance._gameState.GameConfiguration.Name, 
                gameInstance.playerAName, gameInstance.playerBName, gameInstance._gameMode);
        } 
    }

    [BindProperty]
    public string DeleteGameName { get; set; } = string.Empty;
    public IActionResult OnPostDelete()
    {
        if (_gameRepository.DoesSaveGameExist(DeleteGameName))
        {
            _gameRepository.DeleteGame(DeleteGameName);
        }
        return RedirectToPage("ShowGames");
    }

    public IActionResult OnPostMoveGrid()
    {
        HttpContext.Session.SetString("MoveGrid", "Yes");
        return RedirectToPage("GameBoard", new { message = "Select a new position for the grid (topmost, leftmost square)" });
    }

    public Point GetCoordinatesFromString(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return  new Point(-1, -1);
        }
        
        Point output = new Point();
        var coords = input.Split(',');
        output.X = int.Parse(coords[0]);
        output.Y = int.Parse(coords[1]);
        return output;
    }

    public bool IsGameFinished(TicTacTwoBrain gameInstance)
    {
        return gameInstance._gameState.CurrentStatus != EGameStatus.UnFinished;
    }
}