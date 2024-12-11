using DAL;
using Domain;
using GameBrain;
using MenuSystem;


namespace ConsoleApp;

public static class GameController
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;


    public static string StartNewGameLoop(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;

        EGameMode gameMode = ChooseGameMode();
        GetPlayerNames(gameMode, out string playerA, out string playerB);
        
        var chosenConfigShortcut = ChooseConfiguration();

        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }
        
        var chosenConfig = _configRepository.GetConfigurationByName(
            _configRepository.GetConfigurationNames()[configNo]
        );
        
        var gameInstance = new TicTacTwoBrain(chosenConfig, gameMode, playerA, playerB);

        gameInstance.MoveTheGrid(chosenConfig.GridStartPosX, chosenConfig.GridStartPosY);
        
        return GameplayLoop(gameInstance, string.Empty);
    }
    
    public static string LoadExistingGameLoop(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;

        string playerName = AskForPlayerName("one of the player");
        var chosenSaveGameShortcut = ChooseSaveGame(playerName);
        
        int saveGameOption = int.Parse(chosenSaveGameShortcut);
        if (saveGameOption == -1)
        {
            return "";
        }
        
        _gameRepository.LoadGame(_gameRepository.GetSaveGameNames(playerName)[saveGameOption], 
            out GameState chosenSaveGame, out string playerA, out string playerB, out EGameMode gameMode);
        var gameInstance = new TicTacTwoBrain(chosenSaveGame.GameConfiguration, gameMode, playerA, playerB);
        gameInstance._gameState = chosenSaveGame;
        
        return GameplayLoop(gameInstance, string.Empty);
    }

    private static string GameplayLoop(TicTacTwoBrain gameInstance, string message)
    {
        ConsoleUI.Visualizer.DrawBoard(gameInstance);
        WriteMessages(message, "SPECIAL");
        WriteMessages(gameInstance._gameState.NextMoveBy == EGamePiece.X ? $"X's turn ({gameInstance.playerAName})" : 
            $"O's turn ({gameInstance.playerBName})", "TURN");
        CheckForWin(gameInstance);
        ShowMenu(gameInstance);
        
        return string.Empty;
    }

    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = _configRepository.GetConfigurationNames()[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var configMenu = new Menu(EMenuLevel.Secondary,
            "Choose game config",
            configMenuItems, listMenuFlag: true);

        return  configMenu.Run();
    }
    
    private static string ChooseSaveGame(string playerName)
    {

        if (_gameRepository.GetSaveGameNames(playerName).Count == 0)
        {
            Console.WriteLine("No saved games found");
            return "-1";
        }
        
        var saveGameMenuItems = new List<MenuItem>();

        for (var i = 0; i < _gameRepository.GetSaveGameNames(playerName).Count; i++)
        {
            var returnValue = i.ToString();
            saveGameMenuItems.Add(new MenuItem()
            {
                Title = _gameRepository.GetSaveGameNames(playerName)[i],
                Shortcut = (i + 1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        var saveGameMenu = new Menu(EMenuLevel.Secondary,
            "Choose your savegame",
            saveGameMenuItems, listMenuFlag: true);

        return  saveGameMenu.Run();
    }

    private static EGameMode ChooseGameMode()
    {
        EGameMode chosenGameMode = 0;
        var gameModeMenu = new Menu(EMenuLevel.Main,
            "Choose your game mode", [

                new MenuItem()
                {
                    Title = "Player versus Computer",
                    MenuItemAction = () =>
                    {
                        chosenGameMode = EGameMode.PlayerVsAi;
                        return string.Empty;
                    },
                    Shortcut = "C"
                },

                new MenuItem()
                {
                    Title = "Player versus Player",
                    MenuItemAction = () =>
                    {
                        chosenGameMode = EGameMode.PlayerVsAi;
                        return string.Empty;
                    },
                    Shortcut = "P"
                },

                new MenuItem()
                {
                    Title = "Computer versus Computer",
                    MenuItemAction = () =>
                    {
                        chosenGameMode = EGameMode.AiVsAi;
                        return string.Empty;
                    },
                    Shortcut = "A"
                }
            ], listMenuFlag: true);
        gameModeMenu.Run();
        
        return chosenGameMode;
    }
    

    private static int[] AskCoordinates()
    {
        int[] output;
        string input;
        do
        {
            WriteMessages("Enter coordinates <x,y>: ", "ACTION");
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
        int[] coords = new int[2];
        if ((gameInstance._gameMode == EGameMode.PlayerVsAi && gameInstance._gameState.NextMoveBy == EGamePiece.O) ||
            gameInstance._gameMode == EGameMode.AiVsAi)
        {
            coords = gameInstance.GenerateAiMove();
        }
        else
        {
            coords = AskCoordinates();
        }
         

        if (!gameInstance.MakeAMove(coords[0], coords[1]))
        {
            GameplayLoop(gameInstance, "Invalid coordinates");
        }
        
        GameplayLoop(gameInstance, string.Empty);
    }

    private static void MoveGrid(TicTacTwoBrain gameInstance)
    {
        int[] coords = AskCoordinates();

        if (!gameInstance.MoveTheGrid(coords[0], coords[1]))
        {
            GameplayLoop(gameInstance, "Invalid coordinates!");
        }
        
        GameplayLoop(gameInstance, string.Empty);
    }

    private static void MoveExistingPiece(TicTacTwoBrain gameInstance)
    {
        Console.WriteLine("Coordinates of button You want to move:");
        int[] previouscoords = AskCoordinates();
        Console.WriteLine("Where do You want to move it?");
        int[] coords = AskCoordinates();
        
        if (!gameInstance.MoveExistingPiece(coords[0], coords[1], previouscoords[0], previouscoords[1]))
        {
            GameplayLoop(gameInstance, "Invalid coordinates");
        }
        
        GameplayLoop(gameInstance, string.Empty);
    }

    private static void ResetTheGame(TicTacTwoBrain gameInstance)
    {
        gameInstance.ResetGame();
        GameplayLoop(gameInstance, string.Empty);
    }
    private static void ShowMenu(TicTacTwoBrain gameInstance)
    {
        List<MenuItem> actions = new List<MenuItem>();

        if ((gameInstance._gameMode == EGameMode.PlayerVsAi && gameInstance._gameState.NextMoveBy == EGamePiece.O) ||
            gameInstance._gameMode == EGameMode.AiVsAi)
        {
            MakeMove(gameInstance);
        }
        else
        {
            actions.Add(new MenuItem()
            {
                Shortcut = "B",
                Title = "Place a button",
                MenuItemAction = () =>
                {
                    MakeMove(gameInstance);
                    return string.Empty;
                },
            });
        }
        
        actions.Add(new MenuItem()
        {
            Shortcut = "S",
            Title = "Save the current game and exit",
            MenuItemAction = () =>
            {
                _gameRepository.SaveGame(
                    gameInstance.GetGameStateJson(),
                    gameInstance.GetGameConfigName(),
                    gameInstance.playerAName,
                    gameInstance.playerBName,
                    gameInstance._gameMode
                );
                return String.Empty;
            }
        });
        actions.Add(new MenuItem()
        {
            Shortcut = "R",
            Title = "Reset the game",
            MenuItemAction = () =>
            {
                ResetTheGame(gameInstance);
                return string.Empty;
            }
        });

        if (gameInstance.IsGridOrExistingMoveUnlocked())
        {
            actions.Add(new MenuItem()
            {
                Shortcut = "E",
                Title = "Move existing piece",
                MenuItemAction = () =>
                {
                    MoveExistingPiece(gameInstance);
                    return String.Empty;
                },
            });

            actions.Add(new MenuItem()
            {
                Shortcut = "G",
                Title = "Move the grid",
                MenuItemAction = () =>
                {
                    MoveGrid(gameInstance);
                    return string.Empty;
                }
            });
        }
            
        Menu actionMenu = new Menu(
            EMenuLevel.Main, "Choose your action:", actions, listMenuFlag: true);
        actionMenu.Run();
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
               int.TryParse(inputSplit[1].Trim(), out coordinates[1]);
    }

    private static void CheckForWin(TicTacTwoBrain gameInstance)
    {
        gameInstance.FindGridCoordinates(gameInstance, out int gridPosX, out int gridPosY);
        GameWinChecker.CheckForWin(gridPosX, gridPosY, gameInstance._gameState);
        switch (gameInstance._gameState.CurrentStatus)
        {
            case EGameStatus.XWins:
                HandleGameEnd("X won the game! Congrats!");
                break;
            case EGameStatus.OWins:
                HandleGameEnd("O won the game! Congrats!");
                break;
            case EGameStatus.Tie:
                HandleGameEnd("It's a tie! Maybe a new game will sort it out!");
                break;
            case EGameStatus.UnFinished:
                break;
        }
        
        if ((gameInstance._gameMode == EGameMode.AiVsAi || gameInstance._gameMode == EGameMode.PlayerVsAi) &&
            gameInstance.oPieceCount >= gameInstance._gameState.GameConfiguration.GamePiecesPerPlayer)
        {
            HandleGameEnd("It's a tie! Maybe a new game will sort it out!");
        }
    }
    
    private static void HandleGameEnd(string message)
    {
        WriteMessages(message, "SPECIAL");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey(true);
        Console.Clear();
        Menus.MainMenu.Run();
    }

    private static void WriteMessages(string message, string messageFlag)
    {
        switch (messageFlag)
        {
            case "SPECIAL":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case "ACTION":
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                break;
            case "TURN" :
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
        }
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void GetPlayerNames(EGameMode gameMode, out string playerA, out string playerB)
    {
        playerA = playerB = "";
        switch (gameMode)
        {
            case EGameMode.PlayerVsAi:
                playerA = AskForPlayerName("Player A");
                playerB = "AI";
                break;
            case EGameMode.PlayerVsPlayer:
                playerA = AskForPlayerName("Player A");
                playerB = AskForPlayerName("Player B");
                break;
            case EGameMode.AiVsAi:
                playerA = "AI";
                playerB = "AI";
                break;
        }
    }

    private static string AskForPlayerName(string player)
    {
        string playerName = string.Empty;
        do
        {
            WriteMessages($"Please enter {player}'s name: ", "ACTION");
            playerName = Console.ReadLine()!;
        } while (playerName == string.Empty);
        
        playerName = playerName.Trim();
        
        return playerName ?? "";
    }
    
}