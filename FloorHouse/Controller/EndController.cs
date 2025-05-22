using FloorHouse.Model;
using FloorHouse.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FloorHouse.View.MainForm;

namespace FloorHouse.Controller
{
    public interface IEndView
    {
        void CloseEnd();
    }

    public class EndController
    {
        private readonly IEndView _view;
        private MenuForm _menuForm;

        public EndController(IEndView view, MenuForm menuForm)
        {
            _view = view;
            _menuForm = menuForm;
        }

        public void Retry()
        {
            var model = new HouseGameModel(600, 850);
            var mainForm = new MainForm(model, _menuForm);

            mainForm.Show();
            _view.CloseEnd();
        }

        public void GoToMenu()
        {
            var menuForm = new View.MenuForm();
            menuForm.Show();
            _view.CloseEnd();
        }
    }
}
