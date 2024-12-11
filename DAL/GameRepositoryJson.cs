using System.Text.RegularExpressions;
using Domain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public List<string> GetSaveGameNames(string playerName)
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
    public void SaveGame(string jsonStateString, string gameConfigName, string playerA, string playerB, EGameMode gameMode)
    {
        var fileName = FileHelper.BasePath + $"{playerA}_{playerB}_{gameMode}_{gameConfigName}_" +
                       DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss").Replace(":", "") +
                       FileHelper.GameExtension;
        
        File.WriteAllText(fileName, jsonStateString);
    }

    public void LoadGame(string name, out GameState loadedGame, out string playerA, out string playerB, out EGameMode gameMode)
    {
        string pattern = @"^(?<playerA>[^_]+)_(?<playerB>[^_]+)_(?<gameMode>[^_]+)_.+";
        var match = Regex.Match(name, pattern);

        if (!match.Success)
        {
            throw new ArgumentException("Invalid game filename");
        }

        playerA = match.Groups["playerA"].Value;
        playerB = match.Groups["playerB"].Value;

        if (!Enum.TryParse(match.Groups["gameMode"].Value, out gameMode))
        {
            throw new ArgumentException("Invalid game mode in filename");
        }
        
        var saveGameJsonString = File.ReadAllText(FileHelper.BasePath + name + FileHelper.GameExtension);
        loadedGame = System.Text.Json.JsonSerializer.Deserialize<GameState>(saveGameJsonString)!;
    }
}