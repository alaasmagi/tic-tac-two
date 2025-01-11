using Domain;

namespace DAL;

public class GameRepositoryDb(AppDbContext context) : IGameRepository
{
    public void SaveGame(string saveGameName, string jsonStateString, string gameConfigName, string playerA, string playerB, EGameMode gameMode)
    {
        if (string.IsNullOrEmpty(saveGameName))
        {
            saveGameName = GenerateSaveGameName(playerA, playerB, gameMode, gameConfigName);
        }
        
        var existingSaveGame = context.SaveGames.SingleOrDefault(sg => sg.SaveGameName == saveGameName);
        if (existingSaveGame != null)
        {
            existingSaveGame.PlayerAName = playerA;
            existingSaveGame.PlayerBName = playerB;
            existingSaveGame.GameMode = gameMode;
            existingSaveGame.SerializedJsonString = jsonStateString;
        }
        else
        { 
            var saveGame = new SaveGameEntity
            {
                SaveGameName = saveGameName,
                PlayerAName = playerA,
                PlayerBName = playerB,
                GameMode = gameMode,
                SerializedJsonString = jsonStateString
            };

            context.SaveGames.Add(saveGame);
        }
        context.SaveChanges();
    }

    public bool DoesSaveGameExist(string saveGameName)
    {
        return context.SaveGames.Any(s => s.SaveGameName == saveGameName);
    }
    
    public List<string> GetSaveGameNames(string playerName)
    {
        return context.SaveGames
            .Where(s => s.PlayerAName == playerName || s.PlayerBName == playerName || s.GameMode == EGameMode.AiVsAi)
            .Select(s => s.SaveGameName.ToString())
            .ToList();
    }

    public void LoadGame(string saveGameName, out GameState loadedGame, out string playerA, out string playerB, out EGameMode gameMode)
    {
        var saveGame = context.SaveGames
            .FirstOrDefault(s => s.SaveGameName == saveGameName);
        playerA = saveGame!.PlayerAName;
        playerB = saveGame.PlayerBName;
        gameMode = saveGame.GameMode;
        loadedGame = System.Text.Json.JsonSerializer.Deserialize<GameState>(saveGame.SerializedJsonString)!;
    }

    public void DeleteGame(string saveGameName)
    {
        var entityToDelete = context.SaveGames.FirstOrDefault(s => s.SaveGameName == saveGameName); 
        
        if (entityToDelete != null)
        {
            context.SaveGames.Remove(entityToDelete);
            context.SaveChanges();
        }
    }

    public string GenerateSaveGameName(string playerA, string playerB, EGameMode gameMode, string gameConfigName)
    {
        return $"{playerA}_{playerB}_{gameMode}_{gameConfigName}_" +
               DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss").Replace(":", "");
    }
}