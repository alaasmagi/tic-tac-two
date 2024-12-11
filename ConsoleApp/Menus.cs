using DAL;
using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;

    public static void Init(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    public static readonly Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", [
            
            new MenuItem()
            {
                Shortcut = "L",
                Title = "Load Game",
                MenuItemAction = () => GameController.LoadExistingGameLoop(_configRepository, _gameRepository)
            },
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = () => GameController.StartNewGameLoop(_configRepository, _gameRepository)
            },
            new MenuItem()
            {
                Shortcut = "C",
                Title = "Create configuration",
                MenuItemAction = () => ConfigurationController.MainLoop(_configRepository)
            }
        ], listMenuFlag: false);
}