using FloorHouse.Model;

namespace FloorHouse.View
{
    public class ThemedForm : Form
    {
        protected Button ThemeToggleButton;

        public ThemedForm()
        {
            ThemeModel.ThemeChanged += ApplyTheme;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ThemeModel.ThemeChanged -= ApplyTheme;
            base.OnFormClosed(e);
        }

        protected virtual void ApplyTheme(ThemeModel.Theme theme)
        {
            var darkBackColor = Color.FromArgb(30, 30, 30);
            var darkForeColor = Color.White;
            var lightForeColor = Color.Black; // Черный цвет для светлой темы

            BackColor = theme == ThemeModel.Theme.Dark ? darkBackColor : SystemColors.Control;
            ForeColor = theme == ThemeModel.Theme.Dark ? darkForeColor : lightForeColor;

            foreach (Control control in Controls)
            {
                if (control is Button btn)
                {
                    btn.BackColor = theme == ThemeModel.Theme.Dark ? darkBackColor : SystemColors.Control;
                    btn.ForeColor = theme == ThemeModel.Theme.Dark ? darkForeColor : lightForeColor;
                    btn.FlatAppearance.MouseOverBackColor = theme == ThemeModel.Theme.Dark
                        ? Color.FromArgb(50, 50, 50)
                        : SystemColors.ControlLight;
                }
                else if (control is Label lbl)
                {
                    lbl.ForeColor = theme == ThemeModel.Theme.Dark ? darkForeColor : lightForeColor;
                }
            }

            if (ThemeToggleButton != null)
            {
                ThemeToggleButton.Text = theme == ThemeModel.Theme.Light
                    ? "Тёмная тема"
                    : "Светлая тема";
            }
        }

        protected void InitializeThemeButton(int topPosition)
        {
            ThemeToggleButton = new Button
            {
                Text = ThemeModel.CurrentTheme == ThemeModel.Theme.Light ? "Тёмная тема" : "Светлая тема",
                Font = new Font("Press Start 2P", 12),
                Top = topPosition,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            ThemeToggleButton.Click += (s, e) => ThemeModel.ToggleTheme();
            Controls.Add(ThemeToggleButton);
            CenterControl(ThemeToggleButton);
        }

        protected void CenterControl(Control control)
        {
            control.Left = (ClientSize.Width - control.Width) / 2;
        }
    }
}