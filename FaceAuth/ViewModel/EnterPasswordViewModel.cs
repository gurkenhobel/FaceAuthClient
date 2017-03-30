using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FaceAuth.Properties;
using FaceAuth.Util;

namespace FaceAuth.ViewModel
{
    class EnterPasswordViewModel : BaseViewModel
    {

        private string _password;

        private TaskCompletionSource<string> _tcs;

        public ICommand OkCommand { get; private set; }

        public Task<string> Task { get; private set; }



        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public override void Init()
        {
            //configure window
            WindowProperties.Height = 500;
            WindowProperties.Width = 400;
            WindowProperties.CanResize = false;
            WindowProperties.Title = Resources.AppName;

            _tcs = new TaskCompletionSource<string>();
            Task = _tcs.Task;
        }

        public EnterPasswordViewModel()
        {
            OkCommand = new RelayCommand(p =>
            {
                _tcs.SetResult(Password);
            });
        }
    }
}
