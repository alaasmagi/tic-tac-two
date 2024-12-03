using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Domain;

public class GameConfig
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int GamePiecesPerPlayer { get; set; } = 4;
    public int BoardWidth { get; set; } = 5;
    public int BoardHeight { get; set; } = 5;
    public int GridSizeAndWinCondition { get; set; } = 3;
    public int GridStartPosX { get; set; } = 0;
    public int GridStartPosY { get; set; } = 0;
    public int RelocatePiecesAfterMoves { get; set; } = 4;

    public string ToJsonString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
    
}   
