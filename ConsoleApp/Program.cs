using ConsoleApp;
using DAL;
using Microsoft.EntityFrameworkCore;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

var connectionString = $"Data Source=<%location%>{FileHelper.DbExtention}";
connectionString = connectionString.Replace("<%location%>", FileHelper.BasePath);
optionsBuilder.UseSqlite(connectionString);

using var db = new AppDbContext(optionsBuilder.Options);
    
var configRepository = new ConfigRepositoryDb(db);
//var configRepository = new ConfigRepositoryJson();
var gameRepository = new GameRepositoryDb(db);
//var gameRepository = new GameRepositoryJson();


//menu configuration is in Menus.cs
Menus.Init(configRepository, gameRepository);
Menus.MainMenu.Run();