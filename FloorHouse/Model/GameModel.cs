using FloorHouse.Model.Dtos;

namespace FloorHouse.Model
{
    public class GameModel
    {
        // Блоки
        public List<Rectangle> PlacedFloors { get; private set; } = new List<Rectangle>();
        public List<Debris> DebrisList { get; private set; } = new List<Debris>();
        public int FloorWidth { get; } = 140;
        public int FloorHeight { get; } = 110;
        public int NormalShift { get; } = 70;
        public int PerfectStreak { get; private set; } = 0;

        // Камера
        public int CameraOffset { get; private set; }
        private bool cameraLifting = false;
        private int cameraLiftProgress = 0;
        private const int cameraLiftAmount = 110;
        private const int cameraLiftStep = 3;

        // Форма
        private int _currentScore = 0;
        public int FormWidth { get; set; }
        public int FormHeight { get; set; }

        // События
        public event Action StateChanged;
        public event Action GameOver;
        public event Action<int, int> ScoreChanged;
        public event Action CameraMoveRequested;
        public event Action<int> PerfectDrop;

        // Маятник
        private double PendulumAngle = Math.PI / 3;
        private double AngularVelocity;
        private double AngularAcceleration;
        private double Gravity = 0.2;

        // Падение
        public int CurrentX { get; private set; }
        public int CurrentY { get; private set; }
        public bool IsFalling { get; private set; }
        public int FallSpeed { get; private set; }
        public int TargetY { get; private set; }

        // Жизни
        public int Lives { get; private set; } = 3;
        public event Action<int> LivesChanged;

        // Крюк
        public int HookX { get; private set; }
        public int HookY { get; private set; }
        private const double RopeLength = 150;

        // Покачивание башни
        public float TowerAngle { get; private set; } = 0f;
        private float TowerAngularVelocity = 0f;
        private float TowerAngularAcceleration = 0f;
        private const float MaxTowerAngle = 0.2f;
        private const float TowerDamping = 0.96f;

        // Фон
        private ColorDto[] _currentBgColors = new[]
        {
            new ColorDto(240, 128, 128),  // LightCoral
            new ColorDto(255, 255, 224)    // LightYellow
        };
        private PointDto[] _starPositions;
        private int[] _starSizes;
        private Random _rand = new Random();

        public GameModel(int formWidth, int formHeight)
        {
            FormWidth = formWidth;
            FormHeight = formHeight;
            InitializeGame();
            InitializeStars(200);
        }

        public void InitializeGame()
        {
            PlacedFloors.Clear();
            DebrisList.Clear();
            _currentScore = 0;
            PerfectStreak = 0;
            HookX = FormWidth / 2;
            HookY = 20;
            CameraOffset = 0;
            CurrentX = (FormWidth - FloorWidth) / 2;
            CurrentY = FormHeight - FloorHeight - 20;

            FallSpeed = 0;
            IsFalling = false;
            PendulumAngle = Math.PI / 3;
            AngularVelocity = 0;
            AngularAcceleration = 0;
            Gravity = 0.2;

            Lives = 3;

            LivesChanged?.Invoke(Lives);
            ScoreChanged?.Invoke(0, Properties.Settings.Default.HighScore);
            StateChanged?.Invoke();
        }

        public void UpdatePhysics()
        {
            if (IsFalling)
            {
                UpdateFalling();
            }
            else
            {
                UpdatePendulum();
            }

            UpdateCamera();
            UpdateShaking();
            StateChanged?.Invoke();
        }

        private void UpdateFalling()
        {
            FallSpeed += 1;
            CurrentY += FallSpeed;
            TargetY = PlacedFloors.Count == 0
                ? FormHeight - 20
                : PlacedFloors[^1].Y;

            if (CurrentY >= TargetY)
            {
                CurrentY = TargetY;
                IsFalling = false;
                FallSpeed = 0;

                if (PlacedFloors.Count > 0)
                {
                    var last = PlacedFloors[^1];

                    if (CurrentX == last.X)
                    {
                        PerfectStreak++;
                        _currentScore += 3;
                        PerfectDrop?.Invoke(PerfectStreak);
                    }

                    else if (Math.Abs(CurrentX - last.X) <= NormalShift)
                    {
                        PerfectStreak = 0;
                        _currentScore += 1;
                    }

                    else
                    {
                        PerfectStreak = 0;
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
                            TowerAngularVelocity += offset / 500f;
                            ResetCurrentFloor();
                        }
                        return;
                    }
                }
                else
                {
                    _currentScore += 1;
                }

                PlacedFloors.Add(new Rectangle(CurrentX, CurrentY - FloorHeight, FloorWidth, FloorHeight));
                cameraLifting = true;
                cameraLiftProgress = 0;

                if (PlacedFloors.Count % 15 == 0 && Gravity < 2)
                {
                    Gravity += 0.1;
                }

                if (_currentScore > Properties.Settings.Default.HighScore)
                {
                    Properties.Settings.Default.HighScore = _currentScore;
                    Properties.Settings.Default.Save();
                }
                ScoreChanged?.Invoke(_currentScore, Properties.Settings.Default.HighScore);

                CurrentY = FloorHeight + 20 - CameraOffset;
                CheckCameraMove();
            }
        }

        private void UpdatePendulum()
        {
            AngularAcceleration = -Gravity / RopeLength * Math.Sin(PendulumAngle);
            AngularVelocity += AngularAcceleration;
            PendulumAngle += AngularVelocity;
            CurrentX = HookX + (int)(RopeLength * Math.Sin(PendulumAngle)) - FloorWidth / 2;
            CurrentY = HookY + (int)(RopeLength * Math.Cos(PendulumAngle));
        }

        private void UpdateCamera()
        {
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
            }
        }

        private void UpdateShaking()
        {
            TowerAngularAcceleration = -0.01f * TowerAngle;
            TowerAngularVelocity += TowerAngularAcceleration;
            TowerAngularVelocity *= TowerDamping;
            TowerAngle += TowerAngularVelocity;
            TowerAngle = Math.Clamp(TowerAngle, -MaxTowerAngle, MaxTowerAngle);
            UpdateDebris();
        }

        public void DropFloor()
        {
            if (!IsFalling)
            {
                TargetY = PlacedFloors.Count == 0 ? FormHeight - 20 : PlacedFloors[^1].Y;
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

            var lastFloor = PlacedFloors[^1];
            int relativeTop = lastFloor.Y - CameraOffset;

            if (relativeTop <= 350)
                CameraMoveRequested?.Invoke();
        }

        private void ResetCurrentFloor()
        {
            CurrentX = (FormWidth - FloorWidth) / 2;
            CurrentY = FormHeight - FloorHeight - 20 - CameraOffset;
            IsFalling = false;
            FallSpeed = 0;
            PendulumAngle = Math.PI / 3;
            AngularVelocity = 0;
            AngularAcceleration = 0;
        }

        private void SpawnDebris(int x, int y)
        {
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                DebrisList.Add(new Debris
                {
                    X = x + rand.Next(0, FloorWidth),
                    Y = y - CameraOffset,
                    SpeedY = rand.Next(2, 6),
                    Size = rand.Next(5, 10)
                });
            }
        }

        private void UpdateDebris()
        {
            foreach (var d in DebrisList)
                d.Y += d.SpeedY;

            DebrisList.RemoveAll(d => d.Y > FormHeight + 100 + CameraOffset);
        }

        private void InitializeStars(int count)
        {
            _starPositions = new PointDto[count];
            _starSizes = new int[count];

            for (int i = 0; i < count; i++)
            {
                _starPositions[i] = new PointDto(
                    _rand.Next(0, FormWidth * 2),
                    _rand.Next(0, FormHeight * 3));
                _starSizes[i] = _rand.Next(1, 3);
            }
        }

        private ColorDto LerpColor(ColorDto from, ColorDto to, float amount)
        {
            amount = Math.Clamp(amount, 0, 1);
            return new ColorDto(
                (byte)(from.R + (to.R - from.R) * amount),
                (byte)(from.G + (to.G - from.G) * amount),
                (byte)(from.B + (to.B - from.B) * amount),
                (byte)(from.A + (to.A - from.A) * amount)
            );
        }

        public (ColorDto[] colors, PointDto[] starPositions, int[] starSizes, float starOpacity) GetBackgroundData()
        {
            int floorCount = PlacedFloors.Count;
            float progress = Math.Min(floorCount / 100f, 1f);

            // Определяем целевые цвета
            ColorDto[] targetColors = progress switch
            {
                < 0.15f => new[]
                {
                    new ColorDto(240, 128, 128), // LightCoral (RGB: 240,128,128)
                    LerpColor(
                        new ColorDto(255, 255, 224),  // LightYellow
                        new ColorDto(255, 182, 193),  // LightPink
                        progress / 0.15f
                    )
                },
                < 0.25f => new[]
                {
                    LerpColor(
                        new ColorDto(240, 128, 128),  // LightCoral
                        new ColorDto(255, 160, 122),  // LightSalmon
                        (progress - 0.15f) / 0.15f
                    ),
                    LerpColor(
                        new ColorDto(255, 182, 193),  // LightPink
                        new ColorDto(221, 160, 221),  // Plum
                        (progress - 0.15f) / 0.15f
                    )
                },
                < 0.4f => new[]
                {
                    LerpColor(
                        new ColorDto(255, 160, 122),  // LightSalmon
                        new ColorDto(65, 105, 225),    // RoyalBlue
                        (progress - 0.25f) / 0.15f
                    ),
                    LerpColor(
                        new ColorDto(221, 160, 221),   // Plum
                        new ColorDto(123, 104, 238),   // MediumSlateBlue
                        (progress - 0.25f) / 0.15f
                    )
                },
                _ => new[]
                {
                    LerpColor(
                        new ColorDto(65, 105, 225),   // RoyalBlue
                        new ColorDto(0, 0, 139),       // DarkBlue
                        (progress - 0.4f) / 0.15f
                    ),
                    LerpColor(
                        new ColorDto(123, 104, 238),   // MediumSlateBlue
                        new ColorDto(25, 25, 112),     // MidnightBlue
                        (progress - 0.4f) / 0.15f
                    )
                }
            };

            // Плавное изменение текущих цветов
            for (int i = 0; i < 2; i++)
            {
                _currentBgColors[i] = LerpColor(_currentBgColors[i], targetColors[i], 0.05f);
            }

            float starOpacity = Math.Clamp((floorCount - 40) / 40f, 0, 1);

            return (_currentBgColors, _starPositions, _starSizes, starOpacity);
        }
    }
}