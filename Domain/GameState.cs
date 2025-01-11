using System.Drawing;

namespace Domain;

public class GameState
{ 
    public int Id { get; set; }
    public EGamePiece [][] GameBoard { get; set; }
    public EGamePiece NextMoveBy { get; set; } = EGamePiece.O;
    public EGameGrid [][] GameGrid { get; set; }
    public EGameStatus CurrentStatus { get; set; }
    public GameConfig GameConfiguration { get; set; }
    public int XPiecesCount { get; set; }
    public int OPiecesCount { get; set; }

    public List<Point> AiPlacedXPieces { get; set; } = new();
    public List<Point> AiPlacedOPieces { get; set; } = new();

    public GameState(EGamePiece[][] gameBoard, EGameGrid[][] gameGrid, GameConfig gameConfiguration, EGameStatus currentStatus)
    {
        GameBoard = gameBoard;
        GameGrid = gameGrid;
        GameConfiguration = gameConfiguration;
        CurrentStatus = currentStatus;
    }

    public string ToJsonString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
