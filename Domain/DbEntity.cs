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
    public int PlayerAId { get; set; }
    public int PlayerBId { get; set; }
    public string SerializedJsonString { get; set; } = "";
}

public class UserEntity
{
    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string PassHash { get; set; } = "";
}
