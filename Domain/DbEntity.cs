namespace Domain;
public class ConfigurationEntity
{
    public int Id { get; set; }
    public string GameConfigName { get; set; } = "";
    public string SerializedJsonString { get; set; } = "";
}

public class SaveGameEntity
{
    public int Id { get; set; }
    public string SaveGameName { get; set; } = "";
    public string PlayerAName { get; set; } = "";
    public string PlayerBName { get; set; } = "";
    public EGameMode GameMode { get; set; }
    public string SerializedJsonString { get; set; } = "";
}

