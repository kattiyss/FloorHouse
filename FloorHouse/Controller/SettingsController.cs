using FloorHouse.Model;
using FloorHouse.View;

namespace FloorHouse.Controller
{
    public class SettingsController
    {
        private readonly SettingsForm _view;
        private readonly MenuForm _menuView;
        private readonly SettingsModel _model;

        public SettingsController(MenuForm menuForm)
        {
            _menuView = menuForm;
            _model = new SettingsModel();
            _view = new SettingsForm(this);
        }

        public int GetHighScore() => _model.HighScore;

        public void Show()
        {
            _view.Show();
        }

        public void ResetHighScore()
        {
            _model.ResetHighScore();
        }

        public void CloseSettings()
        {
            _view.Close();
            _menuView.Show();
        }
    }
}