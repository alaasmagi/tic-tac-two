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

    public GameConfig GetConfigurationByName(string configName)
    {
        var gameConfig = context.Configurations
            .FirstOrDefault(s => s.GameConfigName == configName);

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

    public bool DoesConfigExist(string configName)
    {
        return context.Configurations.Any(s => s.GameConfigName == configName);
    }
    
    public void DeleteConfig(string configName)
    {
        var entityToDelete = context.Configurations.FirstOrDefault(s => s.GameConfigName == configName); 
        
        if (entityToDelete != null)
        {
            context.Configurations.Remove(entityToDelete);
            context.SaveChanges();
        }
    }
}