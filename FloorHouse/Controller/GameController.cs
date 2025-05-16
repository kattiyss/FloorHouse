using FloorHouse.Model;
using FloorHouse.View;
using System;
using System.Drawing;
using System.Linq;

namespace FloorHouse.Controller
{
    public class GameController
    {
        private readonly HouseGameModel _model;
        private readonly IMainView _view;

        public event Action GameOver;
        public event Action<int, int> UpdateScore;
        public event Action<int> UpdateLives;

        private Image _heartImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\heart.png");
        private Image floorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\floornew.png");
        private Image doorImage = Image.FromFile("C:\\Users\\kattiyss\\source\\repos\\FloorHouse\\doornew.png");

        public GameController(HouseGameModel model, IMainView view)
        {
            _model = model;
            _view = view;

            _model.StateChanged += () => _view.UpdateView();
            _model.GameOver += OnGameOver;
            _model.ScoreChanged += (current, best) => UpdateScore?.Invoke(current, best);
            _model.LivesChanged += lives => UpdateLives?.Invoke(lives);
        }

        public void Initialize()
        {
            _model.InitializeGame();
        }

        public void Update()
        {
            _model.Update();
            _view.MoveCamera(_model.CameraOffset);
        }

        public void DropFloor()
        {
            _model.DropFloor();
        }

        public void SetCameraOffset(int offset)
        {
            _model.SetCameraOffset(offset);
        }

        public void Draw(Graphics g)
        {
            g.TranslateTransform(0, _model.CameraOffset);

            float towerCenterX = _view.GetFormWidth() / 2;
            g.FillRectangle(Brushes.Green, 0, _view.GetFormHeight() - 20, _view.GetFormWidth(), 20);

            int heartSize = 40;
            int spacing = 15;
            int totalWidth = _model.Lives * heartSize + (_model.Lives - 1) * spacing;
            int startX = (_view.GetFormWidth() - totalWidth) / 2;
            int heartY = 10 - _model.CameraOffset;

            for (int i = 0; i < _model.Lives; i++)
                g.DrawImage(_heartImage, startX + i * (heartSize + spacing), heartY, heartSize, heartSize);

            foreach (var floor in _model.PlacedFloors.Select((value, index) => (value, index)))
            {
                if (floor.index == 0)
                    g.DrawImage(doorImage, floor.value);

                g.DrawImage(floorImage, floor.value);
            }

            int drawCurrentY = _model.CurrentY - _model.CameraOffset;
            int floorCenterX = _model.CurrentX + _model.FloorWidth / 2;

            int HookX = _model.FormWidth / 2;
            int HookY = -_model.CameraOffset;

            int pivotX = _model.FormWidth / 2;
            int pivotY = _view.GetFormHeight() - 20 - _model.CameraOffset;

            g.TranslateTransform(pivotX, pivotY);
            g.RotateTransform(_model.TowerAngle * 180f / (float)Math.PI);
            g.TranslateTransform(-pivotX, -pivotY);

            if (!_model.IsFalling)
                g.DrawLine(Pens.Black, HookX, HookY, floorCenterX, drawCurrentY);

            if (_model.PlacedFloors.Count == 0)
                g.DrawImage(doorImage, _model.CurrentX, drawCurrentY, _model.FloorWidth, _model.FloorHeight);

            g.DrawImage(floorImage, _model.CurrentX, drawCurrentY, _model.FloorWidth, _model.FloorHeight);

            foreach (var debris in _model.DebrisList)
            {
                float x = debris.X;
                float y = debris.Y - _model.CameraOffset;
                int size = debris.Size;
                g.FillRectangle(Brushes.Gray, x, y, size, size);
            }
        }

        private void OnGameOver()
        {
            _view.ShowEndForm();
        }
    }
}