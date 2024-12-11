using Domain;
using System.Text.RegularExpressions;

namespace DAL;

public class GameRepositoryDb(AppDbContext context) : IGameRepository
{
    public void SaveGame(string jsonStateString, string gameConfigName, string playerA, string playerB, EGameMode gameMode)
    {
        
        var saveGame = new SaveGameEntity
        {
            SaveGameName =  $"{playerA}_{playerB}_{gameMode}_{gameConfigName}_" +
                            DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss").Replace(":", ""),
            PlayerAName = playerA,
            PlayerBName = playerB,
            SerializedJsonString = jsonStateString
        };
        
        context.SaveGames.Add(saveGame);
        context.SaveChanges();
    }

    public List<string> GetSaveGameNames(string playerName)
    {
        return context.SaveGames
            .Where(s => s.PlayerAName == playerName || s.PlayerBName == playerName)
            .Select(s => s.SaveGameName.ToString())
            .ToList();
    }

    public void LoadGame(string name, out GameState loadedGame, out string playerA, out string playerB, out EGameMode gameMode)
    {
        var saveGame = context.SaveGames
            .FirstOrDefault(s => s.SaveGameName == name);
        playerA = saveGame!.PlayerAName;
        playerB = saveGame!.PlayerBName;
        gameMode = saveGame.GameMode;
        loadedGame = System.Text.Json.JsonSerializer.Deserialize<GameState>(saveGame!.SerializedJsonString)!;
    }
}