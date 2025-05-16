using FloorHouse.View;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloorHouse
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MenuForm());
        }
    }
}
