using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FaceAuth.Properties;
using FaceAuth.Util;

namespace FaceAuth.ViewModel
{
    class LoginViewModel:BaseViewModel
    {
        public ICommand LoginCommand { get; protected set; }
        public ICommand CreateAccountCommand { get; protected set; }


       



        public LoginViewModel()
        {
            //wire commands
            LoginCommand = new RelayCommand(OnLogin);
            CreateAccountCommand = new RelayCommand(OnCreateAccount);
        }


        public override void Init()
        {
            //configure window
            WindowProperties.Height = 500;
            WindowProperties.Width = 400;
            WindowProperties.CanResize = false;
            WindowProperties.Title = Resources.AppName;

        }

        public override void Stop()
        {
            
            BackendConnector.Instance.Stop();
        }

        private void OnCreateAccount(object parameter)
        {
            NavigationController.Instance.ShowView<RegisterViewModel>("register");
        }

        private void OnLogin(object parameter)
        {

            var captureView = NavigationController.Instance.ShowView<FaceCaptureViewModel>("capture");
            captureView.OnSnapshot += async (s, img) =>
            {
                var loadView = NavigationController.Instance.ShowView<LoadingViewModel>("load");
                loadView.Message = "Logging in...";
                NavigationController.Instance.DeleteView<FaceCaptureViewModel>("capture");
                var resp = await BackendConnector.Instance.TryAuth(img);
                if (resp != null && resp.success)
                {
                    var mvm = (MainViewModel) NavigationController.Instance.ViewRoot;
                    mvm.UserName = resp.name;
                    NavigationController.Instance.ShowView<ExplorerViewModel>("home");
                }
                else
                {
                    var msg = NavigationController.Instance.ShowView<MessageViewModel>("login-error");
                    msg.ButtonMessage = "Ok";
                    msg.Message = "Login Error! \nThe App didn't recognize your face";
                    msg.MessageActionCommand = new RelayCommand(p =>
                    {
                        NavigationController.Instance.ShowView<LoginViewModel>("start");
                        NavigationController.Instance.DeleteView<MessageViewModel>("login-error");
                    });
                }
               
            };
        }
    }
}
