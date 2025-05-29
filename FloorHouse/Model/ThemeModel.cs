namespace FloorHouse.Model
{
    public static class ThemeModel
    {
        public enum Theme { Light, Dark }

        public static Theme CurrentTheme { get; private set; }
        public static event Action<Theme> ThemeChanged;

        public static void Initialize()
        {
            CurrentTheme = (Theme)Properties.Settings.Default.Theme;
        }

        public static void ToggleTheme()
        {
            CurrentTheme = CurrentTheme == Theme.Light ? Theme.Dark : Theme.Light;
            Properties.Settings.Default.Theme = (int)CurrentTheme;
            Properties.Settings.Default.Save();
            ThemeChanged?.Invoke(CurrentTheme);
        }
    }
}