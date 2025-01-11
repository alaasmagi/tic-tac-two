using System.Drawing;
using Domain;

namespace GameBrain;

public class AiBrain
{
    public static void AiMove(TicTacTwoBrain gameInstance)
    {
        int randomChoice;
        do
        {
            randomChoice = gameInstance.IsGridOrExistingMoveUnlocked() switch
            {
                true => GenerateRandomNumber(0, 2),
                false => 0
            };
        } while (randomChoice == 0 && ((gameInstance._gameState.NextMoveBy == EGamePiece.X && 
                                        gameInstance._gameState.XPiecesCount >= 
                                        gameInstance._gameState.GameConfiguration.GamePiecesPerPlayer) || 
                                       (gameInstance._gameState.NextMoveBy == EGamePiece.O && 
                                        gameInstance._gameState.OPiecesCount >= 
                                        gameInstance._gameState.GameConfiguration.GamePiecesPerPlayer)));

        switch (randomChoice)
        {
            case 0:
                PlaceButtonRandom(gameInstance);
                break;
            case 1:
                MoveExistingButtonRandom(gameInstance);
                break;
            case 2:
                MoveTheGridRandom(gameInstance);
                break;
        }
    }

    private static void PlaceButtonRandom(TicTacTwoBrain gameInstance)
    {
        Point coordinates;
        EGamePiece whoseTurn = gameInstance._gameState.NextMoveBy;
        
        do
        {
            coordinates = GenerateRandomCoordinates(gameInstance._gameState);
        } while (gameInstance.PlaceAPiece(coordinates) != true);


        switch (whoseTurn)
        {
            case EGamePiece.X:
                gameInstance._gameState.AiPlacedXPieces.Add(new Point(coordinates.X, coordinates.Y));
                break;
            case EGamePiece.O:
                gameInstance._gameState.AiPlacedOPieces.Add(new Point(coordinates.X, coordinates.Y));
                break;
        }
    }

    private static void MoveTheGridRandom(TicTacTwoBrain gameInstance)
    {
        Point coordinates;
        do
        {
            coordinates = GenerateRandomGridCoordinates(gameInstance);
        } while (gameInstance.MoveTheGrid(coordinates) != true);
    }
    
    private static void MoveExistingButtonRandom(TicTacTwoBrain gameInstance)
    { 
        Point previousCoords = new Point();
        Point nextCoords;
        int buttonListIndex;
        EGamePiece whoseTurn = gameInstance._gameState.NextMoveBy;

        switch (whoseTurn)
        {
            case EGamePiece.X:
                do
                {
                    buttonListIndex = GenerateRandomNumber(0, gameInstance._gameState.AiPlacedXPieces.Count - 1);
                    previousCoords.X = gameInstance._gameState.AiPlacedXPieces[buttonListIndex].X;
                    previousCoords.Y = gameInstance._gameState.AiPlacedXPieces[buttonListIndex].Y;
                    nextCoords = GenerateRandomCoordinates(gameInstance._gameState);
                } while (gameInstance.MoveExistingPiece(nextCoords, previousCoords) != true);
                
                gameInstance._gameState.AiPlacedXPieces.RemoveAt(buttonListIndex);
                gameInstance._gameState.AiPlacedXPieces.Add(new Point(nextCoords.X, nextCoords.Y));
                break;
            case EGamePiece.O:
                do
                {
                    buttonListIndex = GenerateRandomNumber(0, gameInstance._gameState.AiPlacedOPieces.Count - 1);
                    previousCoords.X = gameInstance._gameState.AiPlacedOPieces[buttonListIndex].X;
                    previousCoords.Y = gameInstance._gameState.AiPlacedOPieces[buttonListIndex].Y;
                    nextCoords = GenerateRandomCoordinates(gameInstance._gameState);
                } while (gameInstance.MoveExistingPiece(nextCoords, previousCoords) != true);
                
                gameInstance._gameState.AiPlacedOPieces.RemoveAt(buttonListIndex);
                gameInstance._gameState.AiPlacedOPieces.Add(new Point(nextCoords.X, nextCoords.Y));
                break;
        }
    }

    private static Point GenerateRandomCoordinates(GameState gameState)
    {
        Point generatedCoords = new Point();
        do
        {
            generatedCoords.X = GenerateRandomNumber(0 , gameState.GameConfiguration.BoardWidth - 1);
            generatedCoords.Y = GenerateRandomNumber(0, gameState.GameConfiguration.BoardHeight - 1);
        } while (gameState.GameBoard[generatedCoords.X][generatedCoords.Y] != EGamePiece.Empty);
        
        return generatedCoords;
    }

    private static Point GenerateRandomGridCoordinates(TicTacTwoBrain gameInstance)
    {
        Point generatedCoords = new Point();
        do
        {
            generatedCoords.X = GenerateRandomNumber(0, gameInstance._gameState.GameConfiguration.BoardWidth - 
                                                      gameInstance._gameState.GameConfiguration.GridSizeAndWinCondition);
            generatedCoords.Y = GenerateRandomNumber(0, gameInstance._gameState.GameConfiguration.BoardHeight - 
                                                      gameInstance._gameState.GameConfiguration.GridSizeAndWinCondition);
            
        } while (generatedCoords == gameInstance.FindGridCoordinates());
        
        return generatedCoords;
    }

    private static int GenerateRandomNumber(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max + 1);
    }
}