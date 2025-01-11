# Tic-Tac-Two - an enhanced version of popular tic-tac-two game (still under active development)

## Description

* UI language: English
* Development year: **2024**
* Languages and technologies: **Backend: C#, .NET framework, .NET Entity Framework & SQLite  Frontend: Razor pages**
* Rules of the game can be found [here](https://gamescrafters.berkeley.edu/games.php?game=tictactwo)

## How to run

* The game can be started via terminal/cmd by executing "dotnet run" command in source folder.
* It does not need internet connection to run (as it starts on localhost), but for multiplayer mode, the internet connection is necessary.

### Prerequisites

* .NET SDK version 9.0
* SQLite3 (for local database setup)
* Modern web browser

## Explanation of the structure

### Frontend/UI
The software has two UIs:
*  **ConsoleApp**: Old-fashioned looking UI that supports one PC (you can still play 1v1).
*  **WebApp(Razor pages)**:  Modern looking web interface that supports multiple users from other PCs.

### Backend structure
The software has 8 different parts:  
* **Authentication**: As the name says, this part is responsible for authentication in the WebApp UI.
* **ConsoleApp**: This part controls the gameflow in the Console UI.
* **ConsoleUI**: This part is responsible for drawing the board in Console UI.
* **DAL**: Data Access Layer communicates with database or with file system (there is an interface and it can be easily changed between file system and db)
* **Domain**: It contains all of the entities and enums.
* **GameBrain**: This part controls the game flow in general regardless of the UI that is used.
* **MenuSystem**: It controls all of the Console UI's menu elements.
* **WebApp**: This is the second interface for the web users.

### Data Transfer Objects (DTOs) and DB Entities
DTOs are used for communicating with Power Automate (via HTTP) and SQLite database

* **GameConfig:**

```csharp
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
```
  
* **GameState:**

```csharp
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
```

* **Database entities:**

```csharp
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
```

* **Enums:**

```csharp
public enum EGameStatus
{
    XWins,
    OWins,
    Tie,
    UnFinished
}
```

```csharp
public enum EGamePiece
{
    Empty,
    X,
    O,
}
```

```csharp
public enum EGameMode
{
    PlayerVsAi,
    PlayerVsPlayer,
    AiVsAi
}
```

```csharp
public enum EGameGrid
{
    Empty,
    Grid
}
```

### Data management
The application uses two different approaches:  
* File system approach
* Database approach

In both approaches, configuration and gamestate data is stored as serialized JSON string. Data management has an interface and both file system and database approach implement it. So the method can be changed with by changing just the few lines of code.

## Scaling possibilities

### Mobile Application
* Create a dedicated iOS/Android application containing the web UI, while the backend runs on a separate server.

### Cloud Integration
* Host the backend on platforms like AWS, Azure, or GCP for scalability and reliability.
* Store the SQLite database in a cloud-hosted database service such as AWS RDS or Azure SQL Database.
