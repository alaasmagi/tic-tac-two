using Domain;


namespace DAL;

public class ConfigRepositoryHardcoded: IConfigRepository
{
    private List<GameConfig> _gameConfigurations = new List<GameConfig>()
    {
        new GameConfig()
        {
            Name = "Classical"
        },
    };

    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations
            .OrderBy(x => x.Name)
            .Select(config => config.Name)
            .ToList();
    }

    public GameConfig GetConfigurationByName(string configName)
    {
        return _gameConfigurations.Single(c => c.Name == configName);
    }

    public void CreateGameConfig(GameConfig gameConfig)
    {
        throw new NotImplementedException();
    }

    public bool DoesConfigExist(string configName)
    {
        throw new NotImplementedException();
    }

    public void DeleteConfig(string configName)
    {
        throw new NotImplementedException();
    }
}