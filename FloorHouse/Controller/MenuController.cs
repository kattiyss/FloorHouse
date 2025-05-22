using FloorHouse.Model;
using FloorHouse.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FloorHouse.View.MenuForm;
using System.Windows.Forms;

namespace FloorHouse.Controller
{
    public class MenuController
    {
        private readonly IMenuView _view;

        public MenuController(IMenuView view)
        {
            _view = view;
        }

        public void StartGame()
        {
            int formWidth = 600;
            int formHeight = 850;

            var model = new HouseGameModel(formWidth, formHeight);
            var mainForm = new View.MainForm(model, (MenuForm)_view);
            mainForm.Show();
            _view.HideMenu();
        }

        public void Exit()
        {
            Application.Exit();
        }

        public void Settings()
        {
            var settingsController = new SettingsController((MenuForm)_view);
            settingsController.Show();
            _view.HideMenu();
        }
    }
}