using FloorHouse.Model;
using FloorHouse.Model.Dtos;
using FloorHouse.View;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FloorHouse.Controller
{
    public class GameController
    {
        private readonly GameModel _model;
        private readonly IMainView _view;

        public int Lives => _model.Lives;
        public List<Rectangle> PlacedFloors => _model.PlacedFloors;
        public int CurrentX => _model.CurrentX;
        public int CurrentY => _model.CurrentY;
        public bool IsFalling => _model.IsFalling;
        public int FloorWidth => _model.FloorWidth;
        public int FloorHeight => _model.FloorHeight;
        public int FormWidth => _model.FormWidth;
        public float TowerAngle => _model?.TowerAngle ?? 0f;

        public event Action<int, int> UpdateScore;
        public event Action<int> UpdateLives;
        public event Action<int> PerfectDrop;

        public GameController(GameModel model, IMainView view)
        {
            _model = model;
            _view = view;

            _model.StateChanged += () => _view.UpdateView();
            _model.GameOver += OnGameOver;
            _model.ScoreChanged += (current, best) => UpdateScore?.Invoke(current, best);
            _model.LivesChanged += lives => UpdateLives?.Invoke(lives);
            _model.CameraMoveRequested += () => _view.MoveCamera(_model.CameraOffset);
            _model.PerfectDrop += (streak) => PerfectDrop?.Invoke(streak);
            _view.RequestExit += OnRequestExit;

        }

        public void Initialize()
        {
            _model.InitializeGame();
        }

        public void Update()
        {
            _model.UpdatePhysics();
            _view.UpdateDebris(_model.DebrisList);
        }

        public (ColorDto[] colors, PointDto[] starPositions, int[] starSizes, float starOpacity) GetBackgroundData()
        {
            return _model.GetBackgroundData();
        }

        public void DropFloor()
        {
            _model.DropFloor();
        }

        private void OnRequestExit()
        {
            if (_view.ConfirmExit())
                _view.ExitApplication();
        }

        public void SetCameraOffset(int offset)
        {
            _model.SetCameraOffset(offset);
        }

        private void OnGameOver()
        {
            _view.ShowEndForm();
        }
    }
}