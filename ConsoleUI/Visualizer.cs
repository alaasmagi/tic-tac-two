using Domain;
using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        Console.Clear();
        Console.Write("y\\x");
        for (var x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write($" {x}  ");
        }
        Console.WriteLine("");

        
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            Console.Write($"{y}  ");
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x][y]) + " ");
                if (x == gameInstance.DimX - 1) continue;
                if (gameInstance.GameGrid[x][y] == EGameGrid.Grid && gameInstance.GameGrid[x + 1][y] == EGameGrid.Grid){
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("|");
                    Console.ResetColor();
                }
                else {
                    Console.Write("|");
                }
                
            }

            Console.WriteLine();
            if (y == gameInstance.DimY - 1) continue;
            Console.Write("   ");
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                if (gameInstance.GameGrid[x][y] == EGameGrid.Grid && gameInstance.GameGrid[x][y + 1] == EGameGrid.Grid){
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("---");
                    Console.ResetColor();
                    
                    if (x != gameInstance.DimX - 1 && gameInstance.GameGrid[x + 1][y + 1] == EGameGrid.Grid)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("+");
                        Console.ResetColor();
                    }
                    else if (x != gameInstance.DimX - 1)
                    {
                        Console.Write("+");
                    }
                }
                else
                {
                    Console.Write("---");
                    if (x != gameInstance.DimX - 1)
                    {
                        Console.Write("+");
                    }
                }
            }
            Console.WriteLine();
        }
    }
    
    private static string DrawGamePiece(EGamePiece piece) =>
        piece switch
        {
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => " "
        };
}