# Tic-Tac-Two - an enhanced version of popular tic-tac-two game

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

### Data Transfer Objects (DTOs) and DB Entities
DTOs are used for communicating with Power Automate (via HTTP) and SQLite database

* **Request:**

```csharp
public class Request
{
    public DateTime requestTimestamp { get; set; }
    public string? bookingId { get; set; }
    public DateTime bookingStart { get; set; }
    public DateTime bookingEnd { get; set; }
    public string? plcValue { get; set; }
    public string? requestBody { get; set; }
}
```
  
* **RuleEntry:**

```csharp
public class RuleEntry
{
    public string DayOfWeek { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string PlcIds { get; set; }

    public RuleEntry(string dayOfWeek, string startTime, string endTime, string plcIds)
    {
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        PlcIds = plcIds;
    }
}
```

### Data management
The application uses SQLite database and it stores data locally.

Data is separated between 2 tables. Each table has its own use.

* **UL_PLC_DICTIONARY** - stores all PLC data:

```sql
CREATE TABLE "UL_PLC_DICTIONARY" (
	"plc_id"	INTEGER NOT NULL UNIQUE,
	"plc_value"	TEXT NOT NULL UNIQUE,
	"plc_name"	TEXT NOT NULL UNIQUE,
	"class_PC_ip"	TEXT,
	PRIMARY KEY("plc_id")
);
```
  
* **UL_PLC_BOOKINGS** - stores all bookings:

```sql
CREATE TABLE "UL_PLC_BOOKINGS" (
	"plc_id"	INTEGER NOT NULL,
	"booking_id"	TEXT NOT NULL UNIQUE,
	"start"	INTEGER NOT NULL,
	"end"	INTEGER NOT NULL,
	PRIMARY KEY("booking_id")
);
```

### Rule entry
The interface enables teachers/lecturers to book all (or some of) the PLCs for specific timeslots in repeated pattern (ig. timetable). The feature was requested by our supervisor, because it made him easy to book all PLCs for lectures or lab works.

The interface uses rules to implement this feature. Rules are in rules.txt file and are in format:  
`!day of week(abbreviation);HH:mm(start);HH:mm(end);<all PLCs to apply the rule to>`.

## Testing and Debugging
* **Frontend Testing:** Open index.html or tv-view.html in a browser to validate the UI functionality.
* **Backend Testing:** Use tools like Postman to send HTTP requests to the API endpoints and verify responses.
* **Database Debugging:** Open the SQLite database file (plc_booking.db) using tools like DB Browser for SQLite to check stored data manually.

## Developer Guide

### Adding New API Endpoints
* Create a new method in the APIController class to handle the endpoint.
* Add corresponding business logic in the BusinessLogic class.
* Update the SQLite database schema if required and document the changes.

### Adding New Frontend Features
* Modify index.html or tv-view.html and link to new JavaScript or CSS files as needed.
* Use the existing backend endpoints or create new ones for dynamic data fetching.

## Known Limitations
* No support for recurring bookings directly through the interface's own UI.
* Application is not optimized for heavy concurrent user loads (as it currently serves purpose in only one classroom).

## Scaling possibilities

### TV UI Experience Enhancements
* Add features like booking reminders or display additional details about bookings.

### API Enhancements
* Enable the API to send notifications to users if they attempt to use an already occupied PLC without making a reservation.

### Mobile Application
* Create a dedicated iOS/Android application containing the same UIs, while the backend runs on a separate server.

### Cloud Integration
* Host the backend on platforms like AWS, Azure, or GCP for scalability and reliability.
* Store the SQLite database in a cloud-hosted database service such as AWS RDS or Azure SQL Database.
