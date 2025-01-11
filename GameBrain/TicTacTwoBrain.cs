using System.Drawing;
using Domain;

namespace GameBrain;

public class TicTacTwoBrain
{
    public GameState _gameState;
    public readonly EGameMode _gameMode;
    public readonly string playerAName;
    public readonly string playerBName;
    public string saveGameName { get;  set; } = default!;

    public TicTacTwoBrain(GameConfig gameConfiguration, EGameMode gameMode, string playerA, string playerB)
    {
        playerAName = playerA;
        playerBName = playerB;
        _gameMode = gameMode;
        EGameStatus currentStatus = EGameStatus.UnFinished;
        var gameBoard = new EGamePiece[gameConfiguration.BoardWidth][];
        for (int i = 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = new EGamePiece[gameConfiguration.BoardHeight];
        }

        var gameGrid = new EGameGrid[gameConfiguration.BoardWidth][];
        for (int i = 0; i < gameConfiguration.BoardWidth; i++)
        {
            gameGrid[i] = new EGameGrid[gameConfiguration.BoardHeight];
        }

        _gameState = new GameState(
            gameBoard,
            gameGrid,
            gameConfiguration,
            currentStatus
            );
    }

    public void SetGameState(GameState gameState)
    {
        _gameState = gameState;
    }
    
    public string GetGameStateJson()
    {
        return _gameState.ToJsonString();
    }

    public string GetGameConfigName()
    {
        return _gameState.GameConfiguration.Name;
    }

    public EGamePiece[][] GameBoard
    {
        get => GetBoard();
        set => _gameState.GameBoard = value;
    }

    public EGameGrid[][] GameGrid
    {
        get => GetGrid();
        set => _gameState.GameGrid = value;
    }
    
    public int DimX => _gameState.GameBoard.Length;
    public int DimY => _gameState.GameBoard[0].Length;
    private int DimGridXy => _gameState.GameConfiguration.GridSizeAndWinCondition;
    
    private EGamePiece[][] GetBoard()
    {
        var copyOfBoard = new EGamePiece[_gameState.GameBoard.GetLength(0)][];
        for (var x = 0; x < _gameState.GameBoard.Length; x++)
        {
            copyOfBoard[x] = new EGamePiece[_gameState.GameBoard[x].Length];
            for (var y = 0; y < _gameState.GameBoard[x].Length; y++)
            {
                copyOfBoard[x][y] = _gameState.GameBoard[x][y];
            }
        }
        return copyOfBoard;
    }
    
    private EGameGrid[][] GetGrid()
    {
        var copyOfGrid = new EGameGrid[_gameState.GameGrid.GetLength(0)][];
        for (var x = 0; x < _gameState.GameBoard.Length; x++)
        {
            copyOfGrid[x] = new EGameGrid[_gameState.GameGrid[x].Length];
            for (var y = 0; y < _gameState.GameGrid[x].Length; y++)
            {
                copyOfGrid[x][y] = _gameState.GameGrid[x][y];
            }
        }
        return copyOfGrid;
    }

    public bool MoveTheGrid(Point coordinates)
    {

        if (coordinates.X + DimGridXy > DimX || coordinates.Y + DimGridXy > DimY || coordinates.X < 0 || coordinates.Y < 0)
        {
            return false;
        }
        
        ClearGrid();

        int i;
        for (i = 0; i < DimGridXy; i++)
        {
            int j;
            for (j = 0; j < DimGridXy; j++)
            {
                _gameState.GameGrid[coordinates.X + j][coordinates.Y + i] = EGameGrid.Grid;
            }
        }
        FlipTheNextPiece();
        GameWinChecker.CheckForWin(FindGridCoordinates(), _gameState);
        
        return true;
    }

    private void ClearGrid()
    {
        for (var x = 0; x < _gameState.GameGrid.Length; x++)
        {
            for (var y = 0; y < _gameState.GameGrid[x].Length; y++)
            {
                _gameState.GameGrid[x][y] = EGameGrid.Empty;
            }
        }
    }
    
    public bool PlaceAPiece(Point coordinates)
    {
        if (coordinates.X < 0 || coordinates.X >= DimX || coordinates.Y < 0 || coordinates.Y >= DimY || 
            _gameState.GameBoard[coordinates.X][coordinates.Y] != EGamePiece.Empty)
        {
            Console.WriteLine("This place is already occupied");
            return false;
        }

        if (_gameState.NextMoveBy == EGamePiece.O && _gameState.OPiecesCount >= _gameState.GameConfiguration.GamePiecesPerPlayer)
        {
            Console.WriteLine("O is out of pieces");
            return false;
        }

        if (_gameState.NextMoveBy == EGamePiece.X && _gameState.XPiecesCount >= _gameState.GameConfiguration.GamePiecesPerPlayer)
        {
            Console.WriteLine("X is out of pieces");
            return false;
        }
        
        switch (_gameState.NextMoveBy)
        {
            case EGamePiece.X:
                _gameState.XPiecesCount++;
                break;
            case EGamePiece.O:
                _gameState.OPiecesCount++;
                break;
        }
        _gameState.GameBoard[coordinates.X][coordinates.Y] = _gameState.NextMoveBy;
        FlipTheNextPiece();
        GameWinChecker.CheckForWin(FindGridCoordinates(), _gameState);
        
        return true;
    }

    private void FlipTheNextPiece()
    { 
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
    }
    
    public bool MoveExistingPiece(Point coordinates, Point previousCoordinates)
    {
        if (_gameState.XPiecesCount + _gameState.OPiecesCount < 
            _gameState.GameConfiguration.RelocatePiecesAfterMoves)
        {
            Console.WriteLine($"{_gameState.NextMoveBy} you can't move your pieces yet");
            return false;
        }
        
        if (_gameState.GameBoard[previousCoordinates.X][previousCoordinates.Y] != _gameState.NextMoveBy ||
            _gameState.GameBoard[previousCoordinates.X][previousCoordinates.Y] == EGamePiece.Empty ||
            _gameState.GameBoard[coordinates.X][coordinates.Y] != EGamePiece.Empty ||
            coordinates.X < 0 || coordinates.X >= DimX || coordinates.Y < 0 || coordinates.Y >= DimY ||
            previousCoordinates.X < 0 || previousCoordinates.X >= DimX || previousCoordinates.Y < 0 || 
                                                                        previousCoordinates.Y >= DimY)
        {
            Console.WriteLine("Invalid coordinates");
            return false;
        }
        
        _gameState.GameBoard[previousCoordinates.X][previousCoordinates.Y] = EGamePiece.Empty;
        _gameState.GameBoard[coordinates.X][coordinates.Y] = _gameState.NextMoveBy;
        
        FlipTheNextPiece();
        GameWinChecker.CheckForWin(FindGridCoordinates(), _gameState);
        
        return true;
    }
    
    public void ResetGame()
    {
        MoveTheGrid(new Point(0,0));
        _gameState.GameBoard = new EGamePiece[_gameState.GameConfiguration.BoardWidth][];
        for (int i = 0; i <_gameState.GameBoard.Length; i++)
        {
            _gameState.GameBoard[i] = new EGamePiece[_gameState.GameConfiguration.BoardHeight];
        }

        _gameState.NextMoveBy = EGamePiece.O;
        _gameState.XPiecesCount = 0;
        _gameState.OPiecesCount = 0;
        _gameState.CurrentStatus = EGameStatus.UnFinished;
    }

    public Point FindGridCoordinates()
    {
        Point coordinates = new Point();
        for (var i = 0; i < DimX; i++)
        {
            for (var j = 0; j < DimY; j++)
            {
                if (GameGrid[i][j] == EGameGrid.Grid)
                {
                    coordinates.X = i;
                    coordinates.Y = j;
                    return coordinates;
                }
            }
        }
        coordinates.X = 0;
        coordinates.Y = 0;
        return coordinates;
    }

    public bool IsGridOrExistingMoveUnlocked()
    {
        return (_gameState.XPiecesCount + _gameState.OPiecesCount) >
               _gameState.GameConfiguration.RelocatePiecesAfterMoves;
    }
}