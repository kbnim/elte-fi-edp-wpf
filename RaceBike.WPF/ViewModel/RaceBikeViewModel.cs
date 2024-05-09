using RaceBike.Model;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Model.Classes;
using System.Collections.ObjectModel;

namespace RaceBike.ViewModel
{
    public class RaceBikeViewModel : ViewModelBase
    {
        #region Private fields
        private readonly RaceBikeModel _model;
        #endregion

        #region Properties (for the statistics)
        public string LatestBestTime 
        { 
            get { return _model.LatestBestTime.ToString("mm\\:ss\\.ff"); } 
            set { OnPropertyChanged(nameof(LatestBestTime)); } 
        }
        public string CurrentTime
        {
            get { return _model.CurrentTime.ToString("mm\\:ss\\.ff"); }
            set { OnPropertyChanged(nameof(CurrentTime)); }
        } 
        public int CurrentTankLevel
        {
            get { return _model.CurrentTankLevel; }
            set { OnPropertyChanged(nameof(CurrentTankLevel)); }
        }
        public string CurrentSpeed => _model.CurrentSpeed.ToString();
        public SimplePoint BikeLocation => _model.BikeLocation;
        public ObservableCollection<SimplePoint> FuelLocations { get; private set; }
        #endregion

        #region Game commands
        public DelegateCommand KeyCommand_PauseResume { get; private set; }
        public DelegateCommand KeyCommand_MoveLeft { get; private set; }
        public DelegateCommand KeyCommand_MoveRight { get; private set; }
        public DelegateCommand KeyCommand_SpeedUp { get; private set; }
        public DelegateCommand KeyCommand_SlowDown { get; private set; }
        #endregion

        #region Game events
        public event EventHandler? KeyEvent_PauseResume;
        public event EventHandler? KeyEvent_MoveLeft;
        public event EventHandler? KeyEvent_MoveRight;
        public event EventHandler? KeyEvent_SpeedUp;
        public event EventHandler? KeyEvent_SlowDown;
        #endregion

        #region Constructor
        public RaceBikeViewModel(RaceBikeModel model)
        {
            _model = model;
            _model.GameContinues += Model_GameContinues;
            _model.FuelCollectionChanged += Model_FuelCollectionChanged;

            KeyCommand_PauseResume = new DelegateCommand(param => OnPauseResume());
            KeyCommand_MoveLeft = new DelegateCommand(param => OnMoveLeft());
            KeyCommand_MoveRight = new DelegateCommand(param => OnMoveRight());
            KeyCommand_SpeedUp = new DelegateCommand(param => OnSpeedUp());
            KeyCommand_SlowDown = new DelegateCommand(param => OnSlowDown());

            FuelLocations = new ObservableCollection<SimplePoint>(_model.FuelLocations);
        }
        #endregion

        #region Public methods
        public void Reset()
        {
            _model.Reset();
            OnPropertyChanged(nameof(BikeLocation.X));
            FuelLocations.Clear();
            OnPropertyChanged(nameof(FuelLocations));
        }
        #endregion

        #region Private model event handlers
        private void Model_FuelCollectionChanged(object? sender, FuelEventArgs e)
        {
            if (e.IsDeletion)
            {
                FuelLocations.Remove(e.Point);
                OnPropertyChanged(nameof(FuelLocations));
            }
            else
            {
                FuelLocations.Add(e.Point);
                OnPropertyChanged(nameof(FuelLocations));
            }
        }
        private void Model_GameContinues(object? sender, EventArgs e)
        {
            // throw new NotImplementedException();
            OnPropertyChanged(nameof(LatestBestTime));
            OnPropertyChanged(nameof(CurrentTime));
            OnPropertyChanged(nameof(CurrentTankLevel));
            OnPropertyChanged(nameof(CurrentSpeed));

            OnPropertyChanged(nameof(BikeLocation));
            OnPropertyChanged(nameof(FuelLocations));
        }
        #endregion

        #region Private game event handlers
        private void OnPauseResume()
        {
            KeyEvent_PauseResume?.Invoke(this, EventArgs.Empty);
        }

        private void OnMoveLeft()
        {
            KeyEvent_MoveLeft?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(BikeLocation));
        }

        private void OnMoveRight()
        {
            KeyEvent_MoveRight?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(BikeLocation));
        }

        private void OnSpeedUp()
        {
            KeyEvent_SpeedUp?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(CurrentSpeed));
        }

        private void OnSlowDown()
        {
            KeyEvent_SlowDown?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(CurrentSpeed));
        }
        #endregion
    }
}
