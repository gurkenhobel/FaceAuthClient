 using System.Threading.Tasks;
 using System.Windows;
 using FaceAuth.Model;
 using FaceAuth.Properties;
 using FaceAuth.Util;

namespace FaceAuth.ViewModel
{
    class MainViewModel : BaseViewModel, IViewRoot
    {
        private BaseViewModel _currentView;

        private string _userName;

        private bool _showingError;

        public BaseViewModel CurrentView
        {
            get
            {
                return _currentView;
            }

            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        private async Task Start()
        {
            await Task.Run(() =>
            {
                NavigationController.Instance.ShowView<LoginViewModel>("start");


                BackendConnector.Instance.OnConnectionError += (s, e) =>
                {
                    if (_showingError) //to prevent nasty stuff that happens when both close and error events are fired.
                        return;
                    _showingError = true;
                    var msg = NavigationController.Instance.ShowView<MessageViewModel>("connection-error");
                    msg.ButtonMessage = Resources.Reconnect;
                    msg.Message = e;
                    msg.MessageActionCommand = new RelayCommand(p => //buttonclick command
                    {
                        BackendConnector.Instance.Reset(); //reset connection
                        _showingError = false;
                        NavigationController.Instance.DropViews(); //destroy views of previous session
                        Start(); //try reconnect
                    });
                };
                //connect to backend
                BackendConnector.Instance.Init();
            });
        }


        public MainViewModel()
        {
            WindowProperties = new WindowProperties { Height = 500, Width = 400, Title = Resources.AppName, CanResize = true };
            NavigationController.Instance.ViewRoot = this;
            

            Application.Current.MainWindow.Closing += (s, e) =>
            {
                NavigationController.Instance.DropViews();
            };

            Application.Current.MainWindow.Loaded += (s, e) =>
            {
                Start();
            };

        }
    }
}
