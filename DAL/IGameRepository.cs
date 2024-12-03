using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public void SaveGame(string jsonStateString, string gameConfigName);
    public List<string> GetSaveGameNames();
    public GameState LoadGame(string name);
    public string GetSaveConfigName(string gameName);
}