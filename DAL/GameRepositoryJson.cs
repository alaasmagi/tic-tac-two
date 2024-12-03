using GameBrain;
using System.Text.RegularExpressions;
using Domain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public List<string> GetSaveGameNames()
    {
        return Directory
            .GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(
                    Path.GetFileNameWithoutExtension(fullFileName)
                )
            )
            .ToList();
    }
    public void SaveGame(string jsonStateString, string gameConfigName)
    {
        var fileName = FileHelper.BasePath + 
                       gameConfigName + " " + 
                       DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.fffffffzzz").Replace(":", "") + 
                       FileHelper.GameExtension;
        
        System.IO.File.WriteAllText(fileName, jsonStateString);
    }

    public GameState LoadGame(string name)
    {
        var saveGameJsonString = System.IO.File.ReadAllText(FileHelper.BasePath + name + FileHelper.GameExtension);
        GameState loadedGame = System.Text.Json.JsonSerializer.Deserialize<GameState>(saveGameJsonString)!;
        
        return loadedGame!;
    }

    public string GetSaveConfigName(string gameName)
    {
        string pattern = @"(.*?) \d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2}\.\d{7}\+\d{4}";
        
        var match = Regex.Match(gameName, pattern);

        if (!match.Success)
        {
            throw new ArgumentException("Invalid game filename");
        }
        
        return match.Groups[1].Value;
    }
}