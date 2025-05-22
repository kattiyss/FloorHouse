using FloorHouse.Model;
using FloorHouse.View;

namespace FloorHouse;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var menuForm = new MenuForm();
        var model = new HouseGameModel(600, 850);
        var mainForm = new MainForm(model, menuForm);

        Application.Run(menuForm);
    }    
}