using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FaceAuth.Contract;
using FaceAuth.Model;
using FaceAuth.Properties;
using FaceAuth.Util;
using Microsoft.Win32;

namespace FaceAuth.ViewModel
{
    class CreateFileViewModel : BaseViewModel
    {
        private string _name;
        private string _openFileText;
        private bool _encrypt;

        private IOFile _file;

        public Action<LockerObject, IOFile, string> CompleteAction { get; set; }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }



        public string OpenFileText
        {
            get
            {
                return _openFileText;
            }

            set
            {
                _openFileText = value;
                OnPropertyChanged(nameof(OpenFileText));
            }
        }

        public bool Encrypt
        {
            get
            {
                return _encrypt;
            }

            set
            {
                _encrypt = value;
                OnPropertyChanged(nameof(Encrypt));
            }
        }

        public override void Init()
        {
            //configure window
            WindowProperties.Height = 500;
            WindowProperties.Width = 400;
            WindowProperties.CanResize = false;
            WindowProperties.Title = Resources.AppName;

            OpenFileText = Resources.OpenFile;
        }

        public CreateFileViewModel()
        {
            OkCommand = new RelayCommand(p =>
            {
                var pw = ((PasswordBox)p).Password;
                var result = new LockerObject { name = Name, encrypted = Encrypt && !string.IsNullOrEmpty(pw) };
                CompleteAction?.Invoke(result, _file, pw);                                                  
                _file = null;
                OpenFileText = Resources.OpenFile;
            }, DataValid);

            CancelCommand = new RelayCommand(p =>
            {
                CompleteAction?.Invoke(null, null, null);
            });

            OpenFileCommand = new RelayCommand(OpenFile);
        }

        private bool DataValid(object param)
        {
            var nameValid = !string.IsNullOrEmpty(Name);
            var fileValid = _file != null;
            var passwordValid = true;
            return nameValid && fileValid && passwordValid;
        }

        private void OpenFile(object param)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Title = "Open File",
                ValidateNames = true,
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                OpenFileText = Resources.ChangeFile;
                using (var stream = dialog.OpenFile())
                {
                    _file = new IOFile();
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);

                    _file.Data = Convert.ToBase64String(data);
                    _file.Alias = Name;
                    _file.FileName = dialog.SafeFileName;
                    _file.Extension = Path.GetExtension(dialog.FileName);
                }
            }

        }
    }
}
