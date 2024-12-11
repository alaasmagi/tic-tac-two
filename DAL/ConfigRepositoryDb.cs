using Domain;

namespace DAL;

public class ConfigRepositoryDb(AppDbContext context) : IConfigRepository
{
    private void CheckAndCreateInitialConfig()
    {
        if (!context.Configurations.Any())
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

                context.Configurations.Add(config);
            }
            
            context.SaveChanges();
        }
    }

    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();
        return context.Configurations.Select(s => s.GameConfigName.ToString()).ToList();
    }

    public GameConfig GetConfigurationByName(string name)
    {
        var gameConfig = context.Configurations
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

        context.Configurations.Add(config);
        context.SaveChanges();
    }

    public bool DoesConfigExist(string name)
    {
        return context.Configurations.Any(s => s.GameConfigName == name);
    }
}