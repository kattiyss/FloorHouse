using FloorHouse.Model;
using FloorHouse.View;

namespace FloorHouse.Controller
{
    public class MenuController
    {
        private readonly MenuForm _view;

        public MenuController(MenuForm view)
        {
            _view = view;
        }

        public void ToggleTheme()
        {
            ThemeModel.ToggleTheme();
        }

        public void StartGame()
        {
            var model = new GameModel(MenuModel.FormWidth, MenuModel.FormHeight);
            var mainForm = new GameForm(model, _view);
            mainForm.Show();
            _view.HideMenu();
        }

        public void Exit()
        {
            Application.Exit();
        }

        public void Settings()
        {
            var settingsController = new SettingsController(_view);
            settingsController.Show();
            _view.HideMenu();
        }
    }
}