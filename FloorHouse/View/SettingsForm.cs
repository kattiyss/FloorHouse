using FloorHouse.Controller;
using FloorHouse.Model;
using Font = System.Drawing.Font;

namespace FloorHouse.View
{
    public partial class SettingsForm : ThemedForm
    {
        private readonly SettingsController controller;
        private Label lblScore;
        private Button btnReset;
        private Button btnClose;

        public SettingsForm(SettingsController controller)
        {
            this.controller = controller;
            InitializeUI();
            ApplyTheme(ThemeModel.CurrentTheme);
        }

        private void InitializeUI()
        {
            Text = "Настройки";
            Size = new Size(400, 250);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblScore = new Label
            {
                Text = $"Текущий рекорд: {controller.GetHighScore()}",
                Font = new Font("Press Start 2P", 9),
                AutoSize = true,
                Location = new Point(80, 40)
            };
            Controls.Add(lblScore);

            btnReset = new Button
            {
                Text = "Сбросить рекорд",
                Font = new Font("Press Start 2P", 8),
                Size = new Size(280, 40),
                Location = new Point(50, 90)
            };
            btnReset.Click += (s, e) =>
            {
                var result = MessageBox.Show("Вы уверены, что хотите сбросить рекорд?", "Подтверждение", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    controller.ResetHighScore();
                    lblScore.Text = $"Текущий рекорд: 0";
                }
            };
            Controls.Add(btnReset);

            btnClose = new Button
            {
                Text = "Закрыть",
                Font = new Font("Press Start 2P", 8),
                Size = new Size(280, 40),
                Location = new Point(50, 150)
            };
            btnClose.Click += (s, e) => controller.CloseSettings();
            Controls.Add(btnClose);

            InitializeThemeButton(250);
        }
    }
}