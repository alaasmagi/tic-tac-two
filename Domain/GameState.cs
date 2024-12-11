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

    public GameState(EGamePiece[][] gameBoard, EGameGrid[][] gameGrid, GameConfig gameConfiguration, EGameStatus currentStatus, int xPiecesCount, int oPiecesCount)
    {
        GameBoard = gameBoard;
        GameGrid = gameGrid;
        GameConfiguration = gameConfiguration;
        CurrentStatus = currentStatus;
        XPiecesCount = xPiecesCount;
        OPiecesCount = oPiecesCount;
    }

    public string ToJsonString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
