using FloorHouse.Controller;
using FloorHouse.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using static FloorHouse.View.MenuForm;

namespace FloorHouse.View
{
    public partial class MenuForm : Form, IMenuView
    {
        private readonly MenuController _controller;
        private const int FormW = 600;
        private const int FormH = 850;

        public interface IMenuView
        {
            void HideMenu();
        }

        public MenuForm()
        {
            InitializeMenu();
            _controller = new MenuController(this);
        }

        private void InitializeMenu()
        {
            ClientSize = new Size(FormW, FormH);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Floor House";

            var titleLabel = CreateTitleLabel();
            var playButton = CreatePlayButton();
            var settingsButton = CreateSettingsButton();
            var exitButton = CreateExitButton();

            Controls.Add(titleLabel);
            Controls.Add(playButton);
            Controls.Add(settingsButton);
            Controls.Add(exitButton);

            titleLabel.Left = (FormW - titleLabel.Width) / 2;
            playButton.Left = (ClientSize.Width - playButton.Width) / 2;
            settingsButton.Left = (ClientSize.Width - settingsButton.Width) / 2;
            exitButton.Left = (ClientSize.Width - exitButton.Width) / 2;
        }

        private Label CreateTitleLabel()
        {
            return new Label
            {
                Text = "Floor House",
                Font = new Font("Press Start 2P", 25),
                AutoSize = true,
                Top = 50,
                FlatStyle = FlatStyle.Flat
            };
        }

        private Button CreatePlayButton()
        {
            var button = new Button
            {
                Text = "Играть",
                Font = new Font("Press Start 2P", 30),
                AutoSize = true,
                Top = 290,
                FlatStyle = FlatStyle.Flat
            };
            button.Click += (s, e) => _controller.StartGame();
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private Button CreateSettingsButton()
        {
            var button = new Button
            {
                Text = "Настройки",
                Font = new Font("Press Start 2P", 16),
                Top = 370,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat
            };
            button.Click += (s, e) => _controller.Settings();
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private Button CreateExitButton()
        {
            var button = new Button
            {
                Text = "Выход",
                Font = new Font("Press Start 2P", 15),
                Top = 430,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat
            };
            button.Click += (s, e) => _controller.Exit();
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        public void HideMenu()
        {
            Hide();
        }
    }
}