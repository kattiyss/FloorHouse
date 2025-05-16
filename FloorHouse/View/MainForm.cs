using FloorHouse.Controller;
using FloorHouse.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FloorHouse.View
{
    public interface IMainView
    {
        void MoveCamera(int offset);
        void ShowEndForm();
        int GetFormWidth();
        int GetFormHeight();
        void UpdateView();
    }

    public partial class MainForm : Form, IMainView
    {
        private GameController _controller;
        private Timer _gameTimer;
        private MenuForm _menuForm;
        private Label lblCurrent;
        private Label lblBest;
        private Button backButton;
        private HouseGameModel model;

        private int _cameraOffset;

        public MainForm(HouseGameModel model, MenuForm menuForm)
        {
            InitializeComponent();
            _controller = new GameController(model, this);
            _menuForm = menuForm;

            SetupForm();
            SetupLabels();
            SetupTimer();
            SetupKeyboard();

            _controller.Initialize();
            _gameTimer.Start();
        }

        private void InitializeComponent()
        {
            ClientSize = new Size(500, 750);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Floor House";
            DoubleBuffered = true;
            BackColor = Color.White;

            backButton = new Button
            {
                Text = "Назад",
                Font = new Font("Press Start 2P", 10),
                Top = 10,
                Left = 10,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                TabStop = false
            };
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Click += (s, e) =>
            {
            _gameTimer.Stop();
            Hide();
            _menuForm?.Show();
            };
            Controls.Add(backButton);
        }

        private void SetupForm()
        {
            lblCurrent = new Label
            {
                Text = "Счёт: 0",
                Font = new Font("Press Start 2P", 8, FontStyle.Bold),
                AutoSize = true,
                Top = 60,
                Left = 363
            };

            lblBest = new Label
            {
                Text = "Рекорд: " + Properties.Settings.Default.HighScore,
                Font = new Font("Press Start 2P", 8, FontStyle.Bold),
                AutoSize = true,
                Top = 20,
                Left = 363
            };

            Controls.Add(lblCurrent);
            Controls.Add(lblBest);
        }

        private void SetupLabels()
        {
            _controller.UpdateScore += (current, best) =>
            {
                lblCurrent.Text = "Счёт: " + current;
                lblBest.Text = "Рекорд: " + best;
            };
        }

        private void SetupTimer()
        {
            _gameTimer = new Timer { Interval = 1 };
            _gameTimer.Tick += (s, e) => _controller.Update();
        }

        private void SetupKeyboard()
        {
            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Space)
                    _controller.DropFloor();
            };
        }

        public void UpdateView()
        {
            Invalidate();
        }

        public void MoveCamera(int offset)
        {
            _cameraOffset = offset;
        }

        public void ShowEndForm()
        {
            _gameTimer.Stop();
            var endForm = new EndForm(_menuForm);
            endForm.Show();
            Close();
        }

        public int GetFormWidth() => ClientSize.Width;
        public int GetFormHeight() => ClientSize.Height;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _controller?.Draw(e.Graphics);
        }
    }
}