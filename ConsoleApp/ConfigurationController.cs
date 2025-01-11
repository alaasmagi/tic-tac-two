using DAL;
using Domain;

namespace ConsoleApp;

public static class ConfigurationController
{
    private static IConfigRepository _configRepository = default!;
    private const int MinInput = 3;
    private const int MaxInput = 25;
    
    public static string MainLoop(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
        var addConfig = new GameConfig();
        
        
        addConfig.Name = CreateConfigName();
        
        addConfig.BoardHeight = GetInputInt("Board Height", addConfig.BoardHeight, MinInput, MaxInput);
        
        addConfig.BoardWidth = GetInputInt("Board Width", addConfig.BoardWidth, MinInput, MaxInput);
       
        addConfig.GridSizeAndWinCondition = GetInputInt("Grid size and win condition", 
                                                addConfig.GridSizeAndWinCondition, MinInput, addConfig.BoardHeight);
        
        addConfig.GridStartPosX = GetInputInt("Grid start position x coordinate", addConfig.GridStartPosX,
                                                0, addConfig.BoardWidth - addConfig.GridSizeAndWinCondition);
        
        addConfig.GridStartPosY = GetInputInt("Grid start position y coordinate", addConfig.GridStartPosY,
                                                0, addConfig.BoardHeight - addConfig.GridSizeAndWinCondition);
        
        addConfig.GamePiecesPerPlayer = GetInputInt("Number of game pieces per player", addConfig.GamePiecesPerPlayer,
                                                addConfig.GridSizeAndWinCondition, addConfig.BoardHeight * 
                                                    addConfig.BoardWidth / 2 + addConfig.GridSizeAndWinCondition);
        
        addConfig.RelocatePiecesAfterMoves = GetInputInt("Ability to move pieces and grid after n moves (0 to disable)", 
                                                addConfig.RelocatePiecesAfterMoves, 0, 2 * addConfig.GamePiecesPerPlayer);
        
        _configRepository.CreateGameConfig(addConfig);
        return "";
    }

    private static bool ValidateConfigName(string fileName)
    {
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        return fileName.Any(invalidFileNameChars.Contains);
    }

    private static string CreateConfigName()
    {
        string? configName;
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        bool nameCheck = true;

        do
        {
            Console.WriteLine("Enter configuration name: ");
            Console.Write("> ");
            configName = Console.ReadLine();

            if (string.IsNullOrEmpty(configName) || ValidateConfigName(configName))
            {
                Console.WriteLine("Invalid configuration name!");
                Console.WriteLine("Please enter a valid name. Without any of the following characters: ");
                Console.WriteLine($"{String.Join(" ", invalidFileNameChars)}");
                continue;
            }
            
            nameCheck = _configRepository.DoesConfigExist(configName);

            if (nameCheck)
            {
                nameCheck = OverWrite();
            }
            
        } while (nameCheck);
        
        return configName!;
    }

    private static bool OverWrite()
    {
        Console.WriteLine("The configuration name already exists!");
        Console.WriteLine("Overwrite the configuration? (Y/N)");
        Console.Write("> ");

        string answer;

        do
        {
            answer = Console.ReadLine()!.ToUpper();
            if (answer == "y" || answer == "n")
            {
                continue;
            }

            Console.WriteLine("Please enter a valid answer. (Y/N)");
            Console.Write("> ");
        } while (answer != "Y" && answer != "N");

        if (answer != "N")
        {
            return false;
        }

        Console.WriteLine("Please enter new configuration name ");
        return true;
    }

    private static int GetInputInt(string inputType, int defaultValue, int min, int max = int.MaxValue)
    {
        int outputValue;
        string userInput;

        do
        {
            Console.WriteLine($"{inputType} (by default: {defaultValue}, min: {min}, max: {max})");
            Console.Write("> ");
            userInput = Console.ReadLine()!;

            if (string.IsNullOrEmpty(userInput))
            {
                return defaultValue;
            }
            
        } while (!int.TryParse(userInput, out outputValue) || outputValue > max || outputValue < min);
        
        return outputValue;
    }
}