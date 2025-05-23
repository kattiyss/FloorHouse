using System;
using System.Drawing;
using System.Windows.Forms;
using FloorHouse.Controller;
using FloorHouse.Model;

namespace FloorHouse.View
{
    public partial class MenuForm : ThemedForm
    {
        private readonly MenuController _controller;
        private Button _themeToggleButton;

        public MenuForm()
        {
            InitializeMenu();
            _controller = new MenuController(this);
            ApplyTheme(ThemeModel.CurrentTheme);
        }

        private void InitializeMenu()
        {
            ClientSize = new Size(MenuModel.FormWidth, MenuModel.FormHeight);
            StartPosition = FormStartPosition.CenterScreen;
            Text = MenuModel.GameTitle;

            var titleLabel = CreateTitleLabel();
            var playButton = CreatePlayButton();
            var settingsButton = CreateSettingsButton();
            var exitButton = CreateExitButton();

            Controls.Add(titleLabel);
            Controls.Add(playButton);
            Controls.Add(settingsButton);
            Controls.Add(exitButton);

            CenterControl(titleLabel);
            CenterControl(playButton);
            CenterControl(settingsButton);
            CenterControl(exitButton);

            InitializeThemeButton(500);
        }

        private Label CreateTitleLabel()
        {
            return new Label
            {
                Text = MenuModel.GameTitle,
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
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button.Click += (s, e) => _controller.StartGame();
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
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button.Click += (s, e) => _controller.Settings();
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
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            button.Click += (s, e) => _controller.Exit();
            return button;
        }

        private void CenterControl(Control control)
        {
            control.Left = (ClientSize.Width - control.Width) / 2;
        }

        public void HideMenu() => Hide();
    }
}