using FloorHouse.Controller;
using FloorHouse.Model;

namespace FloorHouse.View
{
    public partial class EndForm : ThemedForm
    {
        private readonly EndController _controller;

        public EndForm(MenuForm menuForm)
        {
            InitializeEnd();
            _controller = new EndController(this, menuForm);
            ApplyTheme(ThemeModel.CurrentTheme);
        }

        private void InitializeEnd()
        {
            ClientSize = new Size(EndModel.FormWidth, EndModel.FormHeight);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Game Over";

            var gameOverLabel = CreateGameOverLabel();
            var retryButton = CreateRetryButton();
            var menuButton = CreateMenuButton();

            Controls.Add(gameOverLabel);
            Controls.Add(retryButton);
            Controls.Add(menuButton);

            CenterControl(gameOverLabel);
            CenterControl(retryButton);
            CenterControl(menuButton);
        }

        private Label CreateGameOverLabel()
        {
            return new Label
            {
                Text = EndModel.GameOverText,
                Font = new Font("Press Start 2P", 20),
                AutoSize = true,
                Top = 50,
                FlatStyle = FlatStyle.Flat
            };
        }

        private Button CreateRetryButton()
        {
            var button = new Button
            {
                Text = "Начать заново",
                Font = new Font("Press Start 2P", 20),
                Top = 310,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button.Click += (s, e) => _controller.Retry();
            return button;
        }

        private Button CreateMenuButton()
        {
            var button = new Button
            {
                Text = "Меню",
                Font = new Font("Press Start 2P", 20),
                Top = 380,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button.Click += (s, e) => _controller.GoToMenu();
            return button;
        }

        private void CenterControl(Control control)
        {
            control.Left = (ClientSize.Width - control.Width) / 2;
        }

        public void CloseEnd() => Close();
    }
}