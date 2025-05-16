using System;
using System.Collections.Generic;
using System.Drawing;

namespace FloorHouse.Model
{
    public class HouseGameModel
    {
        //Блоки
        public List<Rectangle> PlacedFloors { get; private set; } = new List<Rectangle>();
        public List<Debris> DebrisList { get; private set; } = new List<Debris>();
        public int FloorWidth { get; } = 140;
        public int FloorHeight { get; } = 110;
        public int NormalShift { get; } = 70;

        //Камера
        public int CameraOffset { get; private set; }
        private bool cameraLifting = false;
        private int cameraLiftProgress = 0;
        private const int cameraLiftAmount = 100;
        private const int cameraLiftStep = 3;

        //Форма
        public int CurrentScore => PlacedFloors.Count;
        public int FormWidth { get; set; }
        public int FormHeight { get; set; }

        //События
        public event Action StateChanged;
        public event Action GameOver;
        public event Action<int, int> ScoreChanged;
        public event Action CameraMoveRequested;

        //Маятник
        private double PendulumAngle = Math.PI / 3;
        private double AngularVelocity;
        private double AngularAcceleration;

        //Падение
        public int CurrentX { get; private set; }
        public int CurrentY { get; private set; }
        public bool IsFalling { get; private set; }
        public int FallSpeed { get; private set; }
        public int TargetY { get; private set; }
        private const double GravityConst = 0.2;

        //Жизни
        public int Lives { get; private set; } = 3;
        public event Action<int> LivesChanged;

        //Крюк
        public int HookX { get; private set; }
        public int HookY { get; private set; }
        private const double RopeLength = 150;

        //Покачивание башни
        public float TowerAngle { get; private set; } = 0f;
        private float TowerAngularVelocity = 0f;
        private float TowerAngularAcceleration = 0f;
        private const float MaxTowerAngle = 0.2f; // около 11 градусов
        private const float TowerDamping = 0.96f;

        public HouseGameModel(int formWidth, int formHeight)
        {
            FormWidth = formWidth;
            FormHeight = formHeight;
            InitializeGame();
        }

        public void InitializeGame()
        {
            PlacedFloors.Clear();
            HookX = FormWidth / 2;
            HookY = 20;
            CameraOffset = 0;
            CameraOffset = 0;
            CurrentX = (FormWidth - FloorWidth) / 2;
            CurrentY = FormHeight - (FormHeight - FloorHeight - 20);

            FallSpeed = 0;
            IsFalling = false;
            PendulumAngle = Math.PI / 3;
            AngularVelocity = 0;
            AngularAcceleration = 0;

            Lives = 3;

            LivesChanged?.Invoke(Lives);
            ScoreChanged?.Invoke(0, Properties.Settings.Default.HighScore);
            StateChanged?.Invoke();
        }

        public void Update()
        {
            if (IsFalling)
            {
                FallSpeed += 1;
                CurrentY += FallSpeed;

                if (CurrentY >= TargetY)
                {
                    CurrentY = TargetY;
                    IsFalling = false;
                    FallSpeed = 0;

                    if (PlacedFloors.Count > 0)
                    {
                        var last = PlacedFloors[PlacedFloors.Count - 1];
                        if (Math.Abs(CurrentX - last.X) > NormalShift)
                        {
                            Lives--;
                            LivesChanged?.Invoke(Lives);
                            SpawnDebris(CurrentX, CurrentY);

                            if (Lives <= 0)
                            {
                                GameOver?.Invoke();
                            }
                            else
                            {
                                float offset = (CurrentX + FloorWidth / 2f) - (last.X + FloorWidth / 2f);
                                TowerAngularVelocity += offset / 500f; // Чем сильнее смещение, тем больше качание
                                ResetCurrentFloor();
                            }
                            return;
                        }

                    }

                    PlacedFloors.Add(new Rectangle(CurrentX, CurrentY - FloorHeight, FloorWidth, FloorHeight));
                    cameraLifting = true;
                    cameraLiftProgress = 0;

                    if (CurrentScore > Properties.Settings.Default.HighScore)
                    {
                        Properties.Settings.Default.HighScore = CurrentScore;
                        Properties.Settings.Default.Save();
                    }
                    ScoreChanged?.Invoke(CurrentScore, Properties.Settings.Default.HighScore);

                    CurrentY = FloorHeight + 20 - CameraOffset;

                    CheckCameraMove();
                }
            }
            else
            {
                AngularAcceleration = -GravityConst / RopeLength * Math.Sin(PendulumAngle);
                AngularVelocity += AngularAcceleration;
                PendulumAngle += AngularVelocity; 
                CurrentX = HookX + (int)(RopeLength * Math.Sin(PendulumAngle)) - FloorWidth / 2;
                CurrentY = HookY + (int)(RopeLength * Math.Cos(PendulumAngle));
            }

            if (cameraLifting && PlacedFloors.Count > 2)
            {
                CameraOffset += cameraLiftStep;
                cameraLiftProgress += cameraLiftStep;

                if (cameraLiftProgress >= cameraLiftAmount)
                {
                    cameraLifting = false;
                    cameraLiftProgress = 0;
                }

                CameraMoveRequested?.Invoke();
                StateChanged?.Invoke();
            }

            TowerAngularAcceleration = -0.01f * TowerAngle; // как бы "пружина" возвращает башню
            TowerAngularVelocity += TowerAngularAcceleration;
            TowerAngularVelocity *= TowerDamping;
            TowerAngle += TowerAngularVelocity;
            TowerAngle = Clamp(TowerAngle, -MaxTowerAngle, MaxTowerAngle);
            UpdateDebris();
            StateChanged?.Invoke();
        }

        public void DropFloor()
        {
            if (!IsFalling)
            {
                TargetY = PlacedFloors.Count == 0 ? FormHeight - 20 : PlacedFloors[PlacedFloors.Count - 1].Y;
                IsFalling = true;
                FallSpeed = 0;

                StateChanged?.Invoke();
            }
        }

        public void SetCameraOffset(int offset)
        {
            CameraOffset = offset;
            StateChanged?.Invoke();
        }

        private void CheckCameraMove()
        {
            if (PlacedFloors.Count == 0) return;

            var lastFloor = PlacedFloors[PlacedFloors.Count - 1];
            int relativeTop = lastFloor.Y - CameraOffset;

            if (relativeTop <= 350)
                CameraMoveRequested?.Invoke();
        }

        private void ResetCurrentFloor()
        {
            CurrentX = (FormWidth - FloorWidth) / 2;
            CurrentY = FormHeight - (FormHeight - FloorHeight - 20) - CameraOffset;
            IsFalling = false;
            FallSpeed = 0;
            PendulumAngle = Math.PI / 3;
            AngularVelocity = 0;
            AngularAcceleration = 0;
        }

        public class Debris
        {
            public float X, Y;
            public float SpeedY;
            public int Size;
        }

        private void SpawnDebris(int x, int y)
        {
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                DebrisList.Add(new Debris
                {
                    X = x + rand.Next(0, FloorWidth),
                    Y = y,
                    SpeedY = rand.Next(2, 6),
                    Size = rand.Next(5, 10)
                });
            }
        }

        private void UpdateDebris()
        {
            foreach (var d in DebrisList)
                d.Y += d.SpeedY;

            DebrisList.RemoveAll(d => d.Y > FormHeight + 100);
        }

        private float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}