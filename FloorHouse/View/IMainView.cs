using FloorHouse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloorHouse.View
{
    public interface IMainView
    {
        void MoveCamera(int offset);
        void ShowEndForm();
        int GetFormWidth();
        int GetFormHeight();
        void UpdateView();
        void UpdateDebris(List<Debris> debrisList);

        event Action RequestExit;
        bool ConfirmExit();
        void ExitApplication();
    }
}
