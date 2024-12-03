using Domain;

namespace DAL;

public class ConfigRepositoryDb:IConfigRepository
{
    private readonly AppDbContext _context;
    public ConfigRepositoryDb()
    {
        var contextFactory = new AppDbContextFactory();
        _context = contextFactory.CreateDbContext([]);
    }

    private void CheckAndCreateInitialConfig()
    {
        if (!_context.Configurations.Any())
        {
            var hardcodedRepo = new ConfigRepositoryHardcoded();
            var optionNames = hardcodedRepo.GetConfigurationNames();

            foreach (var optionName in optionNames)
            {
                var gameOption = hardcodedRepo.GetConfigurationByName(optionName);

                var config = new ConfigurationEntity()
                {
                    GameConfigName = gameOption.Name,
                    SerializedJsonString = System.Text.Json.JsonSerializer.Serialize(gameOption)
                };

                _context.Configurations.Add(config);
            }
            
            _context.SaveChanges();
        }
    }

    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();
        return _context.Configurations.Select(s => s.GameConfigName.ToString()).ToList();
    }

    public GameConfig GetConfigurationByName(string name)
    {
        var gameConfig = _context.Configurations
            .FirstOrDefault(s => s.GameConfigName == name);

        GameConfig loadedConfig = System.Text.Json.JsonSerializer.Deserialize<GameConfig>(gameConfig!.SerializedJsonString)!;

        return loadedConfig;
    }

    public void CreateGameConfig(GameConfig gameConfig)
    {
        var config = new ConfigurationEntity()
        { 
            GameConfigName = gameConfig.Name,
            SerializedJsonString = gameConfig.ToJsonString()
        };

        _context.Configurations.Add(config);
        _context.SaveChanges();
    }

    public bool DoesConfigExist(string name)
    {
        return _context.Configurations.Any(s => s.GameConfigName == name);
    }
}