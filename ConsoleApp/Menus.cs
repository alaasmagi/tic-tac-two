using MenuSystem;

namespace ConsoleApp;

public static class Menus
{
    public static readonly Menu OptionsMenu =
        new Menu(
            EMenuLevel.Secondary,
            "TIC-TAC-TWO Options", [
                new MenuItem()
                {
                    Shortcut = "X",
                    Title = "X Starts",
                    MenuItemAction = DummyMethod
                },
                new MenuItem()
                {
                    Shortcut = "O",
                    Title = "O Starts",
                    MenuItemAction = DummyMethod
                },
                new MenuItem()
                {
                    Shortcut = "C",
                    Title = "Configuration creation",
                    MenuItemAction = OptionsController.MainLoop
                }
            ], listMenuFlag: false);

    public static Menu MainMenu = new Menu(
        EMenuLevel.Main,
        "TIC-TAC-TWO", [
            
            new MenuItem()
            {
                Shortcut = "L",
                Title = "Load Game",
                MenuItemAction = GameController.LoadExistingGameLoop
            },
            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = GameController.StartNewGameLoop
            },
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            }
        ], listMenuFlag: false);

    private static string DummyMethod()
    {
        Console.Write("Just press any key to get out from here! (Any key - as a random choice from keyboard....)");
        Console.ReadKey();
        return "foobar";
    }
}