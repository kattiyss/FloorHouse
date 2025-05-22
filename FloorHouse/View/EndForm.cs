using FloorHouse.Controller;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FloorHouse.View
{
    public partial class EndForm : Form, IEndView
    {
        private EndController _controller;
        private const int FormW = 600;
        private const int FormH = 850;

        public EndForm(MenuForm menuForm)
        {
            InitializeEnd();
            _controller = new EndController(this);
        }

        private void InitializeEnd()
        {
            ClientSize = new Size(FormW, FormH);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Game Over";

            var gameOverLabel = CreateGameOverLabel();
            var retryButton = CreateRetryButton();
            var menuButton = CreateMenuButton();

            Controls.Add(gameOverLabel);
            Controls.Add(retryButton);
            Controls.Add(menuButton);

            retryButton.Left = (ClientSize.Width - retryButton.Width) / 2;
            menuButton.Left = (ClientSize.Width - menuButton.Width) / 2;
        }

        private Label CreateGameOverLabel()
        {
            return new Label
            {
                Text = "Игра окончена",
                Font = new Font("Press Start 2P", 20),
                AutoSize = true,
                Top = 50,
                Left = (FormW - 520),
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
                Left = 65,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat
            };
            button.Click += (s, e) => _controller.Retry();
            button.FlatAppearance.BorderSize = 0;
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
                FlatStyle = FlatStyle.Flat
            };
            button.Click += (s, e) => _controller.GoToMenu();
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        public void CloseEnd()
        {
            this.Close();
        }
    }
}