using System.Configuration;

namespace FloorHouse.Model
{
    public class SettingsModel
    {
        public int HighScore
        {
            get => Properties.Settings.Default.HighScore;
            set
            {
                Properties.Settings.Default.HighScore = value;
                Properties.Settings.Default.Save();
            }
        }

        public void ResetHighScore()
        {
            HighScore = 0;
        }
    }
}