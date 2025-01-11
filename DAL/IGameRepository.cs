using Domain;

namespace DAL;

public interface IGameRepository
{
    public void SaveGame(string saveGameName, string jsonStateString, string gameConfigName, string playerA, string playerB, EGameMode gameMode);
    public bool DoesSaveGameExist(string saveGameName);
    public List<string> GetSaveGameNames(string playerName);
    public void LoadGame(string saveGameName, out GameState loadedGame, out string playerA, out string playerB, out EGameMode gameMode);
    public void DeleteGame(string saveGameName);
    public string GenerateSaveGameName(string playerA, string playerB, EGameMode gameMode, string gameConfigName);
}