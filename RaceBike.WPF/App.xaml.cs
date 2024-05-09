using Microsoft.Win32;
using RaceBike.Model;
using RaceBike.Model.Classes;
using RaceBike.Persistence;
using RaceBike.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RaceBike.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IRaceBikeDataAccess _dataAccess = null!;
        private RaceBikeModel _model = null!;

        private RaceBikeViewModel _mainViewModel = null!;
        private MenuViewModel _menuViewModel = null!;

        private MainWindow _main = null!;
        //private MenuWindow _menu = null!;

        private DispatcherTimer _gameTimer = null!;
        private DispatcherTimer _fuelTimer = null!;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // initialisation of persistence
            _dataAccess = new RaceBikeTxtAccess();

            // initialisation of model
            _model = new RaceBikeModel(_dataAccess);
            _model.GameOver += Model_GameOver;

            // initialisation of view-models
            _mainViewModel = new RaceBikeViewModel(_model);
            _menuViewModel = new MenuViewModel(_model);

            // initialisation of user key events
            _mainViewModel.KeyEvent_PauseResume += ViewModel_KeyEvent_PauseResume;
            _mainViewModel.KeyEvent_MoveLeft += ViewModel_KeyEvent_MoveLeft;
            _mainViewModel.KeyEvent_MoveRight += ViewModel_KeyEvent_MoveRight;
            _mainViewModel.KeyEvent_SpeedUp += ViewModel_KeyEvent_SpeedUp;
            _mainViewModel.KeyEvent_SlowDown += ViewModel_KeyEvent_SlowDown;

            // initialisation of menu button events
            _menuViewModel.ButtonEvent_NewResume += ViewModel_ButtonEvent_NewResume;
            _menuViewModel.ButtonEvent_Load += ViewModel_ButtonEvent_Load;
            _menuViewModel.ButtonEvent_Save += ViewModel_ButtonEvent_Save;
            _menuViewModel.ButtonEvent_Help += ViewModel_ButtonEvent_Help;
            _menuViewModel.ButtonEvent_Quit += ViewModel_ButtonEvent_Quit;

            // initialisation of view (main window)
            _main = new MainWindow()
            {
                DataContext = _mainViewModel,
            };
            _main.Menu.DataContext = _menuViewModel;
            _main.Show();

            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            _gameTimer.Tick += GameTimer_Tick;

            _fuelTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(2000)
            };
            _fuelTimer.Tick += FuelTimer_Tick;
        }

        #region ViewModel events (keystrokes)
        private void ViewModel_KeyEvent_PauseResume(object? sender, EventArgs e)
        {
            if (!_model.IsPaused && !_model.IsGameOver)
            {
                if (_model.CurrentTime > TimeSpan.Zero)
                {
                    _menuViewModel.IsSaveEnabled = true;
                    _menuViewModel.Description01Text = "Time: " + _mainViewModel.CurrentTime;
                    _menuViewModel.Description02Text = string.Empty;
                }

                _gameTimer.Stop();
                _fuelTimer.Stop();
                _model.GameTimePause();
                _main.Menu.Show();
            }
        }

        private void ViewModel_KeyEvent_SlowDown(object? sender, EventArgs e)
        {
            if (!(_model.IsPaused || _model.IsGameOver)) _model.SlowDown();
        }

        private void ViewModel_KeyEvent_SpeedUp(object? sender, EventArgs e)
        {
            if (!(_model.IsPaused || _model.IsGameOver))  _model.SpeedUp();
        }

        private void ViewModel_KeyEvent_MoveRight(object? sender, EventArgs e)
        {
            if (!(_model.IsPaused || _model.IsGameOver))  _model.MoveRight();
        }

        private void ViewModel_KeyEvent_MoveLeft(object? sender, EventArgs e)
        {
            if (!(_model.IsPaused || _model.IsGameOver))  _model.MoveLeft();
        }
        #endregion

        #region Timer events
        private void FuelTimer_Tick(object? sender, EventArgs e)
        {
            _model.GenerateNewFuelItem();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            _model.GameTimeElapsing();
        }
        #endregion

        #region ViewModel events (menu buttons)
        private void ViewModel_ButtonEvent_Quit(object? sender, EventArgs e)
        {
            _main.Menu.Close();
            _main.Close();
        }

        private void ViewModel_ButtonEvent_Help(object? sender, EventArgs e)
        {
            string message = "Move left:\t\tLeft" + Environment.NewLine +
                             "Move right:\t\tRight" + Environment.NewLine +
                             "Speed up:\t\tUp" + Environment.NewLine +
                             "Slow down:\t\tDown" + Environment.NewLine +
                             "Show / hide main menu:\tSpace";
            string caption = "Help";
            MessageBoxButton buttons = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;

            MessageBox.Show(message, caption, buttons, icon);
        }

        private void ViewModel_ButtonEvent_Save(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "RaceBike saving (*.txt)|*.txt",
                    Title = "Load game"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _model.SaveGame(saveFileDialog.FileName);
                }
            }
            catch (RaceBikeDataException)
            {
                string message = "Saving file failed." + Environment.NewLine + "Invalid path or file format.";
                string caption = "Error saving file";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, button, icon);
                _model.Reset();
            }
        }

        private async void ViewModel_ButtonEvent_Load(object? sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "RaceBike saving (*.txt)|*.txt",
                    Title = "Load game"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadGameAsync(openFileDialog.FileName);
                    _menuViewModel.IsSaveEnabled = true;
                }
            }
            catch (RaceBikeDataException)
            {
                string message = "Loading file failed." + Environment.NewLine + "Invalid path or file format.";
                string caption = "Error loading file";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, button, icon);
            }
        }

        private void ViewModel_ButtonEvent_NewResume(object? sender, EventArgs e)
        {
            if (_model.IsPaused)
            {
                if (_model.IsGameOver)
                {
                    _main.Menu.Hide();
                    _menuViewModel.NewResumeText = "Resume";

                    _mainViewModel.Reset();

                    _gameTimer.Start();
                    _fuelTimer.Start();
                    _model.GameTimeResume();
                }
                else
                {
                    _menuViewModel.TitleText = "RaceBike 2000";

                    _main.Menu.Hide();
                    _menuViewModel.NewResumeText = "Resume";
                    _gameTimer.Start();
                    _fuelTimer.Start();
                    _model.GameTimeResume();
                }
            }
        }
        #endregion

        #region Model events
        private void Model_GameOver(object? sender, EventArgs e)
        {
            _gameTimer.Stop();
            _fuelTimer.Stop();
            _model.GameTimePause();
            _menuViewModel.TitleText = "Game over";
            _menuViewModel.NewResumeText = "New";
            _menuViewModel.IsSaveEnabled = true;

            if (_model.LatestBestTime < _model.CurrentTime)
            {
                _menuViewModel.Description01Text = "New record!";
                _menuViewModel.Description02Text = "Time: " + _mainViewModel.CurrentTime;
            }
            else
            {
                _menuViewModel.Description01Text = "Time: " + _mainViewModel.CurrentTime;
                _menuViewModel.Description02Text = "Best: " + _mainViewModel.LatestBestTime;
            }

            _main.Menu.Show();
        }
        #endregion
    }

}
