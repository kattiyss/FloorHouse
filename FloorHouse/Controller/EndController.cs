using FloorHouse.Model;
using FloorHouse.View;

namespace FloorHouse.Controller
{
    public class EndController
    {
        private readonly EndForm _view;
        private readonly MenuForm _menuForm;

        public EndController(EndForm view, MenuForm menuForm)
        {
            _view = view;
            _menuForm = menuForm;
        }

        public void Retry()
        {
            var model = new GameModel(EndModel.FormWidth, EndModel.FormHeight);
            var mainForm = new GameForm(model, _menuForm);
            mainForm.Show();
            _view.CloseEnd();
        }

        public void GoToMenu()
        {
            _menuForm.Show();
            _view.CloseEnd();
        }
    }
}