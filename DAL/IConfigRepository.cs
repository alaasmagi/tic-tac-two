using Domain;

namespace DAL;

public interface IConfigRepository
{
    List<string> GetConfigurationNames();
    GameConfig GetConfigurationByName(string configName);
    void CreateGameConfig(GameConfig gameConfig);
    bool DoesConfigExist(string configName);
    void DeleteConfig(string configName);
}