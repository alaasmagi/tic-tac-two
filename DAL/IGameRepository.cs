using Domain;

namespace DAL;

public interface IGameRepository
{
    public void SaveGame(string jsonStateString, string gameConfigName, string playerA, string playerB, EGameMode gameMode);
    public List<string> GetSaveGameNames(string playerName);
    public void LoadGame(string name, out GameState loadedGame, out string playerA, out string playerB, out EGameMode gameMode);
}