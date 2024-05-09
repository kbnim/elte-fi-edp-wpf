using RaceBike.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RaceBike.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        #region Private fields
        private readonly RaceBikeModel _model;
        #endregion

        #region Properties (for the menu)
        private Visibility _menuVisibility;
        public Visibility MenuVisibility => _menuVisibility;

        private string _titleText;
        public string TitleText
        {
            get { return _titleText; }
            set
            {
                _titleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }

        private string _description01Text;
        public string Description01Text
        {
            get { return _description01Text; }
            set
            {
                _description01Text = value;
                OnPropertyChanged(nameof(Description01Text));
            }
        }

        private string _description02Text;
        public string Description02Text
        {
            get { return _description02Text; }
            set
            {
                _description02Text = value;
                OnPropertyChanged(nameof(Description02Text));
            }
        }

        private string _newResumeText;
        public string NewResumeText
        {
            get { return _newResumeText; }
            set
            {
                _newResumeText = value;
                OnPropertyChanged(nameof(NewResumeText));
            }
        }

        private bool _isSaveEnabled;
        public bool IsSaveEnabled
        {
            get { return _isSaveEnabled; }
            set
            {
                _isSaveEnabled = value;
                OnPropertyChanged(nameof(IsSaveEnabled));
            }
        }
        #endregion

        #region Menu commands
        public DelegateCommand ButtonCommand_NewResume { get; private set; }
        public DelegateCommand ButtonCommand_Load { get; private set; }
        public DelegateCommand ButtonCommand_Save { get; private set; }
        public DelegateCommand ButtonCommand_Help { get; private set; }
        public DelegateCommand ButtonCommand_Quit { get; private set; }
        #endregion

        #region Menu events
        public event EventHandler? ButtonEvent_NewResume;
        public event EventHandler? ButtonEvent_Load;
        public event EventHandler? ButtonEvent_Save;
        public event EventHandler? ButtonEvent_Help;
        public event EventHandler? ButtonEvent_Quit;
        #endregion

        #region Constructor
        public MenuViewModel(RaceBikeModel model)
        {
            _model = model;

            _menuVisibility = Visibility.Visible;
            _titleText = "RaceBike 2000";
            _description01Text = "Press 'space' or";
            _description02Text = "click to start";
            _newResumeText = "New";
            _isSaveEnabled = false;

            ButtonCommand_NewResume = new DelegateCommand(param => OnNewResume());
            ButtonCommand_Load = new DelegateCommand(param => OnLoad());
            ButtonCommand_Save = new DelegateCommand(param => OnSave());
            ButtonCommand_Help = new DelegateCommand(param => OnHelp());
            ButtonCommand_Quit = new DelegateCommand(param => OnQuit());
        }
        #endregion

        #region Public methods
        public void ChangeVisibility()
        {
            if (_model.IsPaused)
            {
                _menuVisibility = Visibility.Visible;
            }
            else
            {
                _menuVisibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Private menu event handlers
        private void OnNewResume()
        {
            ButtonEvent_NewResume?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoad()
        {
            ButtonEvent_Load?.Invoke(this, EventArgs.Empty);
            // OnPropertyChanged(nameof(_model.FuelLocations));
        }

        private void OnSave()
        {
            ButtonEvent_Save?.Invoke(this, EventArgs.Empty);
        }

        private void OnHelp()
        {
            ButtonEvent_Help?.Invoke(this, EventArgs.Empty);
        }

        private void OnQuit()
        {
            ButtonEvent_Quit?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
