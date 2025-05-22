using FloorHouse.Controller;
using FloorHouse.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace FloorHouse.View
{
    public interface IMainView
    {
        void MoveCamera(int offset);
        void ShowEndForm();
        int GetFormWidth();
        int GetFormHeight();
        void UpdateView();
        void UpdateDebris(List<HouseGameModel.Debris> debrisList);

        event Action RequestExit;
        bool ConfirmExit();
        void ExitApplication();
    }

    public partial class MainForm : Form, IMainView
    {
        private GameController _controller;
        private Timer _gameTimer;
        private MenuForm _menuForm;
        private HouseGameModel _model;
        private Label lblCurrent;
        private Label lblBest;
        private Button backButton;
        private int _cameraOffset;
        private List<HouseGameModel.Debris> _debrisList = new List<HouseGameModel.Debris>();
        private Random _rand = new Random();
        public event Action RequestExit;

        private int _perfectStreak = 0;
        private int _perfectDisplayCounter = 0;
        private const int PerfectDisplayDuration = 540;

        private Image _heartImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\heart.png");
        private Image brickFloorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\floorBrick.png");
        private Image brickDoorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\doorBrick.png");
        private Image blueFloorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\floorBlue.png");
        private Image blueDoorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\doorBlue.png");
        private Image yellowFloorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\floorYellow.png");
        private Image yellowDoorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\doorYellow.png");
        private Image stoneFloorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\floorStone.png");
        private Image stoneDoorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\doorStone.png");
        private Image backgroundImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\Themes\\grass.png");
        private Image _currentFloorImage;
        private Image _currentDoorImage;


        public MainForm(HouseGameModel model, MenuForm menuForm)
        {
            InitializeComponent();

            _model = model;
            _model.FormWidth = this.ClientSize.Width;
            _model.FormHeight = this.ClientSize.Height;

            SetThemeImages();

            _controller = new GameController(model, this);
            _menuForm = menuForm;

            SetupForm();
            SetupLabels();
            SetupTimer();
            SetupKeyboard();



            _controller.Initialize();
            _gameTimer.Start();

            _controller.PerfectDrop += (streak) =>
            {
                _perfectStreak = streak;
                _perfectDisplayCounter = PerfectDisplayDuration;
                UpdateView();
            };
        }

        private void InitializeComponent()
        {
            ClientSize = new Size(600, 850);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Floor House";
            DoubleBuffered = true;
            BackColor = Color.White;

            backButton = new Button
            {
                Text = "Назад",
                Font = new Font("Press Start 2P", 12),
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
                Font = new Font("Press Start 2P", 10, FontStyle.Bold),
                AutoSize = true,
                Top = 60,
                Left = 430
            };

            lblBest = new Label
            {
                Text = "Рекорд: " + Properties.Settings.Default.HighScore,
                Font = new Font("Press Start 2P", 10, FontStyle.Bold),
                AutoSize = true,
                Top = 20,
                Left = 430
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
            _gameTimer.Tick += (s, e) =>
            {
                _controller.Update();
                if (_perfectDisplayCounter > 0)
                {
                    _perfectDisplayCounter--;
                    Invalidate();
                }
            };
        }
        private void SetupKeyboard()
        {
            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Space)
                    _controller.DropFloor();
                else if (e.KeyCode == Keys.Escape)
                    RequestExit?.Invoke();
            };
        }

        public bool ConfirmExit()
        {
            return MessageBox.Show("Хотите выйти из игры?", "Подтверждение выхода",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public void ExitApplication() => Application.Exit();

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

        public void UpdateDebris(List<HouseGameModel.Debris> debrisList)
        {
            _debrisList = debrisList;
        }

        private void SetThemeImages()
        {

            var themes = new List<(Image floor, Image door)>()
            {
                (brickFloorImage, brickDoorImage),   // Тема 1: кирпичная
                (blueFloorImage, blueDoorImage),     // Тема 2: синяя
                (yellowFloorImage, yellowDoorImage), // Тема 3: жёлтая
                (stoneFloorImage, stoneDoorImage)    // Тема 4: каменная
            };

            var randomTheme = themes[_rand.Next(themes.Count)];

            _currentFloorImage = randomTheme.floor;
            _currentDoorImage = randomTheme.door;
        }

        protected override void OnResize(EventArgs e)
        {
            if (_model != null)
            {
                _model.FormWidth = this.ClientSize.Width;
                _model.FormHeight = this.ClientSize.Height;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Получаем данные фона
            var (bgColors, starPositions, starSizes, starOpacity) = _controller.GetBackgroundData();

            // Рисуем градиентный фон
            using (var bgBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, GetFormHeight()),
                bgColors[0],
                bgColors[1]))
            {
                g.FillRectangle(bgBrush, 0, 0, GetFormWidth(), GetFormHeight());
            }

            // Рисуем звезды если нужно
            if (starOpacity > 0)
            {
                DrawStars(g, starPositions, starSizes, starOpacity);
            }

            // Draw background
            g.TranslateTransform(0, _cameraOffset);

            // Draw ground
            g.FillRectangle(Brushes.DarkGreen, 0, GetFormHeight() - 21, GetFormWidth(), 21);
            g.DrawImage(backgroundImage, 0, GetFormHeight() - 80, GetFormWidth(), 60);

            // Draw lives
            int heartSize = 60;
            int spacing = 15;
            int totalWidth = _controller.Lives * heartSize + (_controller.Lives - 1) * spacing;
            int startX = (GetFormWidth() - totalWidth) / 2;
            int heartY = 10 - _cameraOffset;

            for (int i = 0; i < _controller.Lives; i++)
                g.DrawImage(_heartImage, startX + i * (heartSize + spacing), heartY, heartSize, heartSize);

            // Draw floors
            foreach (var floor in _controller.PlacedFloors.Select((value, index) => (value, index)))
            {
                if (floor.index == 0)
                    g.DrawImage(_currentDoorImage, floor.value);
                else
                    g.DrawImage(_currentFloorImage, floor.value);
            }

            // Draw current floor
            int drawCurrentY = _controller.CurrentY - _cameraOffset;
            int floorCenterX = _controller.CurrentX + _controller.FloorWidth / 2;

            // Draw rope
            int HookX = GetFormWidth() / 2;
            int HookY = -_cameraOffset;

            // Draw debris
            foreach (var debris in _debrisList)
            {
                float x = debris.X;
                float y = debris.Y - _cameraOffset;
                int size = debris.Size;
                g.FillRectangle(Brushes.Gray, x, y, size, size);
            }

            int pivotX = GetFormWidth() / 2;
            int pivotY = GetFormHeight() - 20 - _cameraOffset;

            g.TranslateTransform(pivotX, pivotY);
            g.RotateTransform(_controller.TowerAngle * 180f / (float)Math.PI);
            g.TranslateTransform(-pivotX, -pivotY);

            if (!_controller.IsFalling)
                g.DrawLine(Pens.Black, HookX, HookY, floorCenterX, drawCurrentY);

            // Draw current floor image
            if (_controller.PlacedFloors.Count == 0)
                g.DrawImage(_currentDoorImage, _controller.CurrentX, drawCurrentY, _controller.FloorWidth, _controller.FloorHeight);
            else
                g.DrawImage(_currentFloorImage, _controller.CurrentX, drawCurrentY, _controller.FloorWidth, _controller.FloorHeight);
            
            if (_perfectDisplayCounter > 0 && _perfectStreak > 0)
            {
                string text = $"ИДЕАЛЬНО x{_perfectStreak}! +{_perfectStreak * 3}";
                SizeF textSize = g.MeasureString(text, new Font("Press Start 2P", 16, FontStyle.Bold));
                float x = (GetFormWidth() - textSize.Width) / 2;
                float y = 100 - _cameraOffset;

                // Тень текста
                g.DrawString(text, new Font("Press Start 2P", 16, FontStyle.Bold), Brushes.Black, x + 2, y + 2);
                // Основной текст
                g.DrawString(text, new Font("Press Start 2P", 16, FontStyle.Bold), Brushes.Gold, x, y);

                _perfectDisplayCounter--;
            }
        }

        private void DrawStars(Graphics g, Point[] positions, int[] sizes, float opacity)
        {
            int visibleStars = (int)(positions.Length * opacity);
            using (var starBrush = new SolidBrush(Color.FromArgb((int)(200 * opacity), Color.White)))
            {
                for (int i = 0; i < visibleStars; i++)
                {
                    int x = positions[i].X;
                    int y = (positions[i].Y - (int)(_cameraOffset * 0.3f)) % (GetFormHeight() * 2);
                    if (y < -10) y += GetFormHeight() * 2; // Чтобы звезды появлялись сверху

                    g.FillEllipse(starBrush, x, y, sizes[i], sizes[i]);
                }
            }
        }
    }
}