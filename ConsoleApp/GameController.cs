using System.Runtime.InteropServices.JavaScript;
using DAL;
using Domain;
using GameBrain;
using MenuSystem;

namespace ConsoleApp;

public static class GameController
{
    private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryDb();
    private static readonly IGameRepository GameRepository = new GameRepositoryDb();

    public static string StartNewGameLoop()
    {
        var chosenConfigShortcut = ChooseConfiguration();

        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        
        var chosenConfig = ConfigRepository.GetConfigurationByName(
            ConfigRepository.GetConfigurationNames()[configNo]
        );
        
        var gameInstance = new TicTacTwoBrain(chosenConfig);

        gameInstance.MoveTheGrid(chosenConfig.GridStartPosX, chosenConfig.GridStartPosY);
        
        return GameplayLoop(gameInstance);
    }
    
    public static string LoadExistingGameLoop()
    {
        var chosenSaveGameShortcut = ChooseSaveGame();
        
        int saveGameOption = int.Parse(chosenSaveGameShortcut);
        if (saveGameOption == -1)
        {
            return "";
        }
       
        var chosenSaveGame = GameRepository.LoadGame(GameRepository.GetSaveGameNames()[saveGameOption]);
        var chosenConfigName = GameRepository.GetSaveConfigName(GameRepository.GetSaveGameNames()[saveGameOption]);
        var chosenConfig = ConfigRepository.GetConfigurationByName(chosenConfigName);
        var gameInstance = new TicTacTwoBrain(chosenConfig);
        gameInstance._gameState = chosenSaveGame;
    
        return GameplayLoop(gameInstance);
    }

    public static string GameplayLoop(TicTacTwoBrain gameInstance)
    {
        var winConditionCheck = new GameWinChecker();
        do
        {
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            
            switch (ChooseNextAction(gameInstance._gameState))
            {
                case "B":
                    MakeMove(gameInstance);
                    
                    continue;
                case "E":
                    MoveExistingPiece(gameInstance);
                    continue;
                case "G":
                    MoveGrid(gameInstance);
                    continue;
                case "S":
                    GameRepository.SaveGame(
                        gameInstance.GetGameStateJson(),
                        gameInstance.GetGameConfigName()
                    );
                    return "";
                case "R":
                    gameInstance.ResetGame();
                    continue;
            }
            
        } while (true);
    }

    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < ConfigRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = ConfigRepository.GetConfigurationNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var configMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose game config",
            configMenuItems, listMenuFlag: true);

        return  configMenu.Run();
    }
    
    private static string ChooseSaveGame()
    {

        if (GameRepository.GetSaveGameNames().Count == 0)
        {
            Console.WriteLine("No saved games found");
            return "-1";
        }
        
        var saveGameMenuItems = new List<MenuItem>();

        for (var i = 0; i < GameRepository.GetSaveGameNames().Count; i++)
        {
            var returnValue = i.ToString();
            saveGameMenuItems.Add(new MenuItem()
            {
                Title = GameRepository.GetSaveGameNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var saveGameMenu = new Menu(EMenuLevel.Secondary,
            "TIC-TAC-TWO - choose your savegame",
            saveGameMenuItems, listMenuFlag: true);

        return  saveGameMenu.Run();
    }

    private static EGameMode ChooseGameMode()
    {
        EGameMode chosenGameMode;
        while (true)
        {
            Console.WriteLine("Choose game mode:");
            Console.WriteLine("1 - Player vs Computer");
            Console.WriteLine("2 - Player vs Player");
            Console.WriteLine("3 - Computer vs Computer");

            var input = Console.ReadLine();
            
            if (int.TryParse(input, out int modeNumber) &&
                Enum.IsDefined(typeof(EGameMode), modeNumber - 1))
            {
                chosenGameMode = (EGameMode)(modeNumber - 1);
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please choose a valid game mode (1-3).");
            }
        }

        return chosenGameMode;
    }
    

    private static int[] AskCoordinates()
    {
        int[] output = new int[2];
        string input;
        do
        {
            Console.Write("Enter coordinates <x,y>: ");
            input = Console.ReadLine()!;
            if (!ValidateCoordinates(input, out output))
            {
                Console.WriteLine("Invalid coordinates");
            }
        } while (!ValidateCoordinates(input, out output));
        
        return output;
    }

    private static void MakeMove(TicTacTwoBrain gameInstance)
    {
        int[] coords = AskCoordinates();

        if (!gameInstance.MakeAMove(coords[0], coords[1]))
        {
            Console.WriteLine("Invalid coordinates");
        }
    }

    private static void MoveGrid(TicTacTwoBrain gameInstance)
    {
        int[] coords = AskCoordinates();

        if (!gameInstance.MoveTheGrid(coords[0], coords[1]))
        {
            Console.WriteLine("Invalid coordinates");
        }
    }

    private static void MoveExistingPiece(TicTacTwoBrain gameInstance)
    {
        Console.WriteLine("Coordinates of button You want to move:");
        int[] previouscoords = AskCoordinates();
        Console.WriteLine("Where do You want to move it?");
        int[] coords = AskCoordinates();
        
        if (!gameInstance.MoveExistingPiece(coords[0], coords[1], previouscoords[0], previouscoords[1]))
        {
            Console.WriteLine("Invalid coordinates");
        }
    }

    private static string ChooseNextAction(GameState gameInstance)
    {
        Console.WriteLine();
        Console.WriteLine("======================");
        if (gameInstance.NextMoveBy == EGamePiece.X)
        {
            Console.WriteLine("Player X next move:");
        }
        else
        {
            Console.WriteLine("Player O next move:");
        }
        Console.Write("Write B to move button, " +
                      "E to move existing piece or " +
                      "G to move the grid or " +
                      "S to save & exit or " +
                      "R to reset: ");
        var input = Console.ReadLine()!;

        return input.ToUpper();
    }

    private static bool ValidateCoordinates(string input, out int[] coordinates)
    {
        coordinates = new int[2];
        if (input.Length != 3 || !input.Contains(','))
        {
            return false;
        }
        var inputSplit = input.Split(",");
        return int.TryParse(inputSplit[0].Trim(), out coordinates[0]) &&
               int.TryParse(inputSplit[1].Trim(), out coordinates[1]) == true;
    }

    private static void CheckForWin(TicTacTwoBrain gameInstance)
    {
        gameInstance.FindGridCoordinates(gameInstance, out int gridPosX, out int gridPosY);
        GameWinChecker.CheckForWin(gridPosX, gridPosY, gameInstance._gameState);
        switch (gameInstance._gameState.CurrentStatus)
        {
            case EGameStatus.XWins:
                Console.WriteLine("X won the game! Congrats!");
                Console.WriteLine("Press any key to return to main menu...");
                break;
            case EGameStatus.OWins:
                Console.WriteLine("O won the game! Congrats!");
                Console.WriteLine("Press any key to return to main menu...");
                break;
            case EGameStatus.Tie:
                Console.WriteLine("It's a tie! Maybe new game will sort it out!");
                Console.WriteLine("Press any key to return to main menu...");
                break;
            case EGameStatus.UnFinished:
                break;
        }
    }
}