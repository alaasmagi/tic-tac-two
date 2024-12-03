using Domain;

namespace GameBrain;

public class GameWinChecker
{
    public static void CheckForWin(int gridPosX, int gridPosY, GameState gameState)
     {
         var winCondition = gameState.GameConfiguration.GridSizeAndWinCondition;
         
         CheckRows(gridPosX, gridPosY, gameState, winCondition);
         if (gameState.CurrentStatus is EGameStatus.Tie or EGameStatus.OWins or EGameStatus.XWins)
         {
             return;
         }
         
         CheckColumns(gridPosX, gridPosY, gameState, winCondition);
         if (gameState.CurrentStatus is EGameStatus.Tie or EGameStatus.OWins or EGameStatus.XWins)
         {
             return;
         }
         
         CheckDiagonals(gridPosX, gridPosY, gameState, winCondition);
     }
     private static void CheckRows(int x, int y, GameState gameState, int winCon)
     {
         for (var z = 0; z < winCon; z++)
         {
             var xCount = 0;
             var oCount = 0;
             for (var q = 0; q < winCon; q++)
             {
                 switch (gameState.GameBoard[x + q][y + z])
                 {
                     case EGamePiece.O:
                         oCount++;
                         break;
                     case EGamePiece.X:
                         xCount++;
                         break;
                     case EGamePiece.Empty:
                         break;
                 }

                 IfWall(xCount, oCount, gameState, winCon);
             }
         }
     }

     private static void CheckColumns(int x, int y, GameState gameState, int winCon)
     {
         for (var z = 0; z < winCon; z++)
         {
             var xCount = 0;
             var oCount = 0;
             for (var q = 0; q < winCon; q++)
             {
                 switch (gameState.GameBoard[x + z][y + q])
                 {
                     case EGamePiece.O:
                         oCount++;
                         break;
                     case EGamePiece.X:
                         xCount++;
                         break;
                     case EGamePiece.Empty:
                         break;
                 }

                 IfWall(xCount, oCount, gameState, winCon);
             }
         }
     }

     private static void CheckDiagonals(int x, int y, GameState gameState, int winCon)
     {
         Diag1(x, y, gameState, winCon);

         Diag2(x, y, gameState, winCon);
     }

     private static void Diag1(int x, int y, GameState gameState, int winCon)
     {
         var oCount = 0;
         var xCount = 0;
         for (var i = 0; i < winCon; i++)
         {
             switch (gameState.GameBoard[x + i][y + i])
             {
                 case EGamePiece.O:
                     oCount++;
                     break;
                 case EGamePiece.X:
                     xCount++;
                     break;
                 case EGamePiece.Empty:
                     break;
             }

             if (xCount == winCon)
             {
                 gameState.CurrentStatus = EGameStatus.XWins;
             }
             else if (oCount == winCon)
             {
                 gameState.CurrentStatus = EGameStatus.OWins;
             }
         }
     }

     private static void Diag2(int x, int y, GameState gameState, int winCon)
     {
         var oCount = 0;
         var xCount = 0;
         var i = 0;
         for (var j = winCon - 1; j >= 0; j--)
         {
             switch (gameState.GameBoard[x + j][y + i])
             {
                 case EGamePiece.O:
                     oCount++;
                     break;
                 case EGamePiece.X:
                     xCount++;
                     break;
                 case EGamePiece.Empty:
                     break;
             }

             if (xCount == winCon)
             {
                 gameState.CurrentStatus = EGameStatus.XWins;
             }
             else if (oCount == winCon)
             {
                 gameState.CurrentStatus = EGameStatus.OWins;
             }

             i++;
         }
     }

     private static void IfWall(int xCount, int oCount, GameState gameState, int winCon)
     {
         if (xCount == winCon && gameState.CurrentStatus != EGameStatus.OWins)
         {
             gameState.CurrentStatus = EGameStatus.XWins;
         }
         else if (oCount == winCon && gameState.CurrentStatus != EGameStatus.XWins)
         {
             gameState.CurrentStatus = EGameStatus.OWins;
         }
         else if (xCount == winCon && gameState.CurrentStatus == EGameStatus.OWins)
         {
             gameState.CurrentStatus = EGameStatus.Tie;
         }
         else if (oCount == winCon && gameState.CurrentStatus == EGameStatus.XWins)
         {
             gameState.CurrentStatus = EGameStatus.Tie;
         }
     }
}