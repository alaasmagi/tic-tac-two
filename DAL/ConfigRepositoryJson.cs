using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryJson : IConfigRepository
{
    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();

        return Directory
            .GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
            .Select(fullFileName =>
                Path.GetFileNameWithoutExtension(
                    Path.GetFileNameWithoutExtension(fullFileName)
                )
            )
            .ToList();
    }

    public GameConfig GetConfigurationByName(string name)
    {
        var configJsonStr = System.IO.File.ReadAllText(FileHelper.BasePath + name + FileHelper.ConfigExtension);
        var config = System.Text.Json.JsonSerializer.Deserialize<GameConfig>(configJsonStr);
        return config!;
    }

    private void CheckAndCreateInitialConfig()
    {
        if (!System.IO.Directory.Exists(FileHelper.BasePath))
        {
            System.IO.Directory.CreateDirectory(FileHelper.BasePath);
        }

        var data = System.IO.Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension).ToList();
        if (data.Count == 0)
        {
            var hardcodedRepo = new ConfigRepositoryHardcoded();
            var optionNames = hardcodedRepo.GetConfigurationNames();
            foreach (var optionName in optionNames)
            {
                var gameOption = hardcodedRepo.GetConfigurationByName(optionName);
                var optionJsonStr = System.Text.Json.JsonSerializer.Serialize(gameOption);
                System.IO.File.WriteAllText(FileHelper.BasePath + gameOption.Name + FileHelper.ConfigExtension, optionJsonStr);
            }
        }
    }

    public void CreateGameConfig(GameConfig gameConfig)
    {
        var fileLocation = Path.Combine(FileHelper.BasePath, gameConfig.Name + FileHelper.ConfigExtension);
        
        var gameConfigJsonStr = System.Text.Json.JsonSerializer.Serialize(gameConfig);
        File.WriteAllText(fileLocation, gameConfigJsonStr);
        Console.WriteLine($"New game configuration created successfully: {gameConfig.Name}");
    }

    public bool DoesConfigExist(string name)
    {
        return File.Exists(FileHelper.BasePath + name + FileHelper.ConfigExtension);
    }
}
