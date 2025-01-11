using System.Text.RegularExpressions;
using Domain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public List<string> GetSaveGameNames(string playerName)
    {

        return Directory
            .GetFiles(FileHelper.BasePath, "*" + FileHelper.GameExtension).Where(fullFileName => 
                Path.GetFileNameWithoutExtension(fullFileName).Contains(playerName) || 
                Path.GetFileNameWithoutExtension(fullFileName).Contains("AiVsAi")).Select(fullFileName =>
                Path.GetFileNameWithoutExtension(
                    Path.GetFileNameWithoutExtension(fullFileName)
                )
            )
            .ToList();
    }
    public void SaveGame(string saveGameName, string jsonStateString, string gameConfigName, string playerA, string playerB, EGameMode gameMode)
    {
        if (string.IsNullOrEmpty(saveGameName))
        {
            saveGameName = GenerateSaveGameName(playerA, playerB, gameMode, gameConfigName);
        }
         
        if (!saveGameName.EndsWith(FileHelper.GameExtension))
        {
            saveGameName += FileHelper.GameExtension;
        }
        
        var filePath = Path.Combine(FileHelper.BasePath, saveGameName);
        File.WriteAllText(filePath, jsonStateString);
    }
    
    public bool DoesSaveGameExist(string saveGameName)
    {
        return File.Exists(FileHelper.BasePath + saveGameName + FileHelper.ConfigExtension);
    }


    public void LoadGame(string saveGameName, out GameState loadedGame, out string playerA, out string playerB, out EGameMode gameMode)
    {
        string pattern = @"^(?<playerA>[^_]+)_(?<playerB>[^_]+)_(?<gameMode>[^_]+)_.+";
        var match = Regex.Match(saveGameName, pattern);

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
        
        var saveGameJsonString = File.ReadAllText(FileHelper.BasePath + saveGameName + FileHelper.GameExtension);
        loadedGame = System.Text.Json.JsonSerializer.Deserialize<GameState>(saveGameJsonString)!;
    }

    public void DeleteGame(string saveGameName)
    {
        File.Delete(FileHelper.BasePath + saveGameName + FileHelper.GameExtension);
    }

    public string GenerateSaveGameName(string playerA, string playerB, EGameMode gameMode, string gameConfigName)
    {
        return $"{playerA}_{playerB}_{gameMode}_{gameConfigName}_" +
               DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss").Replace(":", "");

    }
}