using FloorHouse.Model;

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
