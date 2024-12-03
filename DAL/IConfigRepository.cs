using Domain;
using GameBrain;

namespace DAL;

public interface IConfigRepository
{
    List<string> GetConfigurationNames();
    GameConfig GetConfigurationByName(string name);
    void CreateGameConfig(GameConfig gameConfig);
    bool DoesConfigExist(string name);
}