using Domain;
using System.Text.RegularExpressions;

namespace DAL;

public class GameRepositoryDb:IGameRepository
{
    private readonly AppDbContext _context;
    public GameRepositoryDb()
    {
        var contextFactory = new AppDbContextFactory();
        _context = contextFactory.CreateDbContext([]);
    }
    
    public void SaveGame(string jsonStateString, string gameConfigName)
    {
        var saveGame = new SaveGameEntity
        {
            SaveGameName =  gameConfigName + " " + 
                            DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.fffffffzzz").Replace(":", ""),
            SerializedJsonString = jsonStateString
        };

        _context.SaveGames.Add(saveGame);
        _context.SaveChanges();
    }

    public List<string> GetSaveGameNames()
    {
        return _context.SaveGames.Select(s => s.SaveGameName.ToString()).ToList();
    }

    public GameState LoadGame(string name)
    {
        var saveGame = _context.SaveGames
            .FirstOrDefault(s => s.SaveGameName == name);
        
        GameState loadedGame = System.Text.Json.JsonSerializer.Deserialize<GameState>(saveGame!.SerializedJsonString)!;

        return loadedGame;
    }

    public string GetSaveConfigName(string gameName)
    {
        var saveGame = _context.SaveGames
            .FirstOrDefault(s => s.SaveGameName == gameName);
        
        string pattern = @"(.*?) \d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2}\.\d{7}\+\d{4}";
        
        var match = Regex.Match(saveGame!.SaveGameName, pattern);

        if (!match.Success)
        {
            throw new ArgumentException("Invalid game name");
        }
        return match.Groups[1].Value;
    }
}