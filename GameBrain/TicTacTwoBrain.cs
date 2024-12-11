using Domain;
using System.Threading;


namespace GameBrain;

public class TicTacTwoBrain
{
    public GameState _gameState;
    public EGameMode _gameMode;
    public int xPieceCount;
    public int oPieceCount;
    public string playerAName;
    public string playerBName;
    private int gamePieceCount;

    public int[] gameGridPos = { 0, 0 };

    public TicTacTwoBrain(GameConfig gameConfiguration, EGameMode gameMode, string playerA, string playerB)
    {
        playerAName = playerA;
        playerBName = playerB;
        gamePieceCount = gameConfiguration.GamePiecesPerPlayer;
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
            currentStatus,
            xPieceCount,
            oPieceCount
            );
    }

    public void SetGameState(GameState gameState)
    {
        _gameState = gameState;

        xPieceCount = _gameState.XPiecesCount;
        oPieceCount = _gameState.OPiecesCount;
        
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
        private set => _gameState.GameBoard = value;
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

    public bool MoveTheGrid(int x, int y)
    {
        if (x + DimGridXy > DimX || y + DimGridXy > DimY || x < 0 || y < 0)
        {
            return false;
        }
        
        ClearGrid();
        
        int i, j;
        for (i = 0; i < DimGridXy; i++)
        {
            for (j = 0; j < DimGridXy; j++)
            {
                _gameState.GameGrid[x + j][y + i] = EGameGrid.Grid;
            }
        }

        FlipTheNextPiece();
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
    
    public bool MakeAMove(int x, int y)
    {
        if (x < 0 || x >= DimX || y < 0 || y >= DimY || _gameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            Console.WriteLine("This place is already occupied");
            return false;
        }

        if (_gameState.NextMoveBy == EGamePiece.O && oPieceCount >= _gameState.GameConfiguration.GamePiecesPerPlayer)
        {
            Console.WriteLine("O is out of pieces");
            return false;
        }

        if (_gameState.NextMoveBy == EGamePiece.X && xPieceCount >= _gameState.GameConfiguration.GamePiecesPerPlayer)
        {
            Console.WriteLine("X is out of pieces");
            return false;
        }
        
        switch (_gameState.NextMoveBy)
        {
            case EGamePiece.X:
                xPieceCount++;
                break;
            case EGamePiece.O:
                oPieceCount++;
                break;
        }
        _gameState.GameBoard[x][y] = _gameState.NextMoveBy;
        FlipTheNextPiece();
        
        return true;
    }

    public void FlipTheNextPiece()
    { 
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
    }
    
    public bool MoveExistingPiece(int x, int y, int previousX, int previousY)
    {
        if ((xPieceCount + oPieceCount) < 
            _gameState.GameConfiguration.RelocatePiecesAfterMoves)
        {
            Console.WriteLine($"{_gameState.NextMoveBy} you can't move your pieces yet");
            return false;
        }
        
        if (_gameState.GameBoard[previousX][previousY] != _gameState.NextMoveBy ||
            _gameState.GameBoard[previousX][previousY] == EGamePiece.Empty ||
            _gameState.GameBoard[x][y] != EGamePiece.Empty ||
            x < 0 || x >= DimX || y < 0 || y >= DimY ||
            previousX < 0 || previousX >= DimX || previousY < 0 || previousY >= DimY)
        {
            Console.WriteLine("Invalid coordinates");
            return false;
        }
        
        _gameState.GameBoard[previousX][previousY] = EGamePiece.Empty;
        _gameState.GameBoard[x][y] = _gameState.NextMoveBy;
        
        FlipTheNextPiece();
        return true;
    }
    
    public void ResetGame()
    {
        MoveTheGrid(0, 0);
        _gameState.GameBoard = new EGamePiece[_gameState.GameConfiguration.BoardWidth][];
        for (int i = 0; i <_gameState.GameBoard.Length; i++)
        {
            _gameState.GameBoard[i] = new EGamePiece[_gameState.GameConfiguration.BoardHeight];
        }

        _gameState.NextMoveBy = EGamePiece.O;
        xPieceCount = 0;
        oPieceCount = 0;
    }
    
    public bool FindGridCoordinates(TicTacTwoBrain gameInstance, out int x, out int y)
    {
        for (var i = 0; i < gameInstance.DimX; i++)
        {
            for (var j = 0; j < gameInstance.DimY; j++)
            {
                if (gameInstance.GameGrid[i][j] == EGameGrid.Grid)
                {
                    x = i;
                    y = j;
                    return true;
                }
            }
        }
        x = 0;
        y = 0;
        return false;
    }
    
    public int[] GenerateAiMove()
    {
        Random random = new Random();
        int[] generatedCoords = new int[2];
        do
        {
            generatedCoords[0] = random.Next(0 , _gameState.GameConfiguration.BoardHeight - 1);
            generatedCoords[1] = random.Next(0, _gameState.GameConfiguration.BoardHeight - 1);
        } while (_gameState.GameBoard[generatedCoords[0]][generatedCoords[1]] != EGamePiece.Empty);
        
        Thread.Sleep(3 * 1000);
        return generatedCoords;
    }

    public bool IsGridOrExistingMoveUnlocked()
    {
        return (xPieceCount + oPieceCount) >
               _gameState.GameConfiguration.RelocatePiecesAfterMoves;
    }
}