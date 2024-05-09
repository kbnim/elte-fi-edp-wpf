using RaceBike.Model.Classes;
using RaceBike.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model
{
    public class RaceBikeModel
    {
        #region Properties
        //public TimeSpan LatestBestTime { get => _statistics.LatestBestTime; set => _statistics.LatestBestTime = value; }
        //public int LoadedBikePosition => _statistics.BikePosition;
        //public ImmutableArray<SimplePoint> LoadedFuels => _statistics.FuelPositions.ToImmutableArray();
        public TimeSpan LatestBestTime { get; private set; }
        public TimeSpan CurrentTime => _stopwatch.Elapsed;
        public int CurrentTankLevel => CalculateTankLevelPercent();
        public ImmutableSpeed CurrentSpeed => _bike.Speed;
        public bool IsPaused => !_stopwatch.IsRunning;
        public bool IsGameOver => _bike.IsOutOfGas;
        public SimplePoint BikeLocation => _bike.Location;
        public IEnumerable<SimplePoint> FuelLocations => _fuels.Select(fuel => fuel.Location);
        #endregion

        #region Private fields
        private readonly IRaceBikeDataAccess? _dataAccess;
        private readonly Bike _bike;
        private readonly List<Fuel> _fuels;
        private readonly SmartStopwatch _stopwatch;
        private readonly object _lock;
        #endregion

        #region Constants
        private const int COURSE_WIDTH = 480; // actual width of the window in pixels
        private const int COURSE_HEIGHT = 600; // actual height of the window in pixels
        private const int ENTITY_SIZE = 40; // width and height of each moving entity in the game in pixels
        #endregion

        #region Events
        public event EventHandler<FuelEventArgs>? FuelCollectionChanged;
        public event EventHandler? GameContinues;
        public event EventHandler? GameOver;
        #endregion

        #region Constructor
        public RaceBikeModel(IRaceBikeDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _bike = new Bike(COURSE_WIDTH, COURSE_HEIGHT, ENTITY_SIZE);
            _fuels = new List<Fuel>();
            _stopwatch = new SmartStopwatch();
            _lock = new object();
        }
        #endregion

        #region Public methods
        public void Reset()
        {
            _bike.Reset();
            _fuels.Clear();
            _stopwatch.Restart(); // megj.: .Reset() != .Restart()
        }

        public async Task LoadGameAsync(string path)
        {
            if (_dataAccess is null)
            {
                throw new InvalidOperationException("No data access was provided.");
            }

            // emptying out storage to load new data
            // _fuels.Clear();

            while (_fuels.Count != 0)
            {
                OnFuelCollectionChanged(true, _fuels[_fuels.Count - 1].Location);
                _fuels.RemoveAt(_fuels.Count - 1);
            }

            RaceBikeFile fileContents = await _dataAccess.LoadAsync(path);

            _stopwatch.Add(fileContents.LatestBestTime);
            LatestBestTime = fileContents.LatestBestTime;
            _bike.SetSpeed(fileContents.Speed);
            
            foreach (GameField entity in fileContents.Entities)
            {
                switch (entity.Type)
                {
                    case FieldType.Bike:
                        {
                            _bike.Location.X = entity.Row; 
                            _bike.Location.Y = entity.Column; 
                            break;
                        }
                    case FieldType.Fuel:
                        {
                            Fuel fuel = new(COURSE_WIDTH, COURSE_HEIGHT, ENTITY_SIZE);
                            fuel.Location.X = entity.Row;
                            fuel.Location.Y = entity.Column;
                            _fuels.Add(fuel);
                            OnFuelCollectionChanged(false, fuel.Location);
                            break;
                        }
                    default: break;
                }
            }
        }

        public void SaveGame(string path)
        {
            Queue<GameField> entities = new();
            entities.Enqueue(new GameField(_bike.Location, FieldType.Bike));

            foreach (Fuel fuel in _fuels)
            {
                entities.Enqueue(new GameField(fuel.Location, FieldType.Fuel));
            }

            RaceBikeFile file = new(LatestBestTime, CurrentSpeed, entities);
            _dataAccess?.SaveAsync(path, file);
        }

        public void GameTimeElapsing()
        {
            if (_bike.IsOutOfGas)
            {
                _stopwatch.Stop();

                if (_stopwatch.Elapsed > LatestBestTime)
                {
                    LatestBestTime = _stopwatch.Elapsed;
                }

                OnGameOver();
            }
            else
            {
                lock (_lock)
                {
                    Fuel? lostItem = null;
                    Fuel? nearbyItem = null;

                    foreach (Fuel fuel in _fuels)
                    {
                        fuel.MoveDownwards(_bike.Speed);

                        if (_bike.IsFuelNear(fuel))
                        {
                            IncreaseTankLevel();
                            nearbyItem = fuel;
                        }

                        if (fuel.IsOutsideOfWindow)
                        {
                            lostItem = fuel;
                        }
                    }

                    if (nearbyItem is not null)
                    {
                        OnFuelCollectionChanged(true, nearbyItem.Location);
                        _fuels.Remove(nearbyItem);
                    }

                    if (lostItem is not null)
                    {
                        OnFuelCollectionChanged(true, lostItem.Location);
                        _fuels.Remove(lostItem);
                    }

                    DecreaseTankLevel();
                    OnGameContinues();
                }
            }
        }

        public void GameTimePause()
        {
            if (!IsPaused)
            {
                _stopwatch.Stop();

                if (_stopwatch.Elapsed > LatestBestTime)
                {
                    LatestBestTime = _stopwatch.Elapsed;
                }
            }
        }

        public void GameTimeResume()
        {
            if (IsPaused)
            {
                _stopwatch.Start();
            }
        }

        public void IncreaseTankLevel()
        {
            if (_fuels.Count != 0)
            {
                // Peek() != Dequeue(); mivel Dequeue() jelenti a pop() műveletet
                _bike.IncreaseTankLevel(_fuels[0]); 
            }
        }

        public void GenerateNewFuelItem() 
        {
            lock (_lock)
            {
                // Enqueue() == push()
                _fuels.Add(new Fuel(COURSE_WIDTH, COURSE_HEIGHT, ENTITY_SIZE));
                OnFuelCollectionChanged(false, _fuels[_fuels.Count - 1].Location);
            }
        }

        public void SpeedUp() { _bike.SpeedUp(); }

        public void SlowDown() { _bike.SlowDown(); }

        public void MoveLeft()
        {
            _bike.MoveLeft();
        }

        public void MoveRight()
        {
            _bike.MoveRight();
        }
        #endregion

        #region Private methods
        private void DecreaseTankLevel() { _bike.DecreaseTankLevel(); }

        private int CalculateTankLevelPercent()
        {
            double percent = (double)_bike.TankLevel / _bike.MaxCapacity * 100;
            return (int)percent;
        }
        #endregion

        #region Private event methods
        private void OnFuelCollectionChanged(bool isDeletion, SimplePoint point = null!)
        {
            FuelCollectionChanged?.Invoke(this, new FuelEventArgs(isDeletion, point));
        }

        private void OnGameContinues()
        {
            GameContinues?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
