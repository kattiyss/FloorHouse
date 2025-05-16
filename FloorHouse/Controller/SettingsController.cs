using FloorHouse.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloorHouse.Controller
{
    public class SettingsController
    {
        private readonly SettingsForm _view;
        private readonly MenuForm _menuView;

        public SettingsController(MenuForm menuForm)
        {
            _menuView = menuForm;
            _view = new SettingsForm(this);
        }

        public void Show()
        {
            _view.Show();
        }

        public void ResetHighScore()
        {
            Properties.Settings.Default.HighScore = 0;
            Properties.Settings.Default.Save();
            MessageBox.Show("Рекорд сброшен!");
        }

        public void CloseSettings()
        {
            _view.Close();
            _menuView.Show();
        }
    }
}
