using ConsoleApp;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// menu configuration is in Menus.cs
Menus.MainMenu.Run();

var services = new ServiceCollection();

var connectionString = $"Data Source={FileHelper.BasePath}app.db";

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

services.AddScoped<IGameRepository, GameRepositoryDb>();

var serviceProvider = services.BuildServiceProvider();
var gameRepository = serviceProvider.GetService<IGameRepository>();


