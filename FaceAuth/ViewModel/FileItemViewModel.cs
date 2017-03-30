using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FaceAuth.Annotations;
using FaceAuth.Model;
using FaceAuth.Properties;
using FaceAuth.Util;
using Microsoft.Win32;

namespace FaceAuth.ViewModel
{
    class FileItemViewModel: INotifyPropertyChanged
    {
        #region propertychanged impl
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string Name { get; set; }

        public ICommand SelectCommand { get; private set; }
        public ICommand DownloadCommand { get; private set; }


        public event EventHandler OnSelect;



        private Bitmap _image;
        private LockerObject _fileInfo;
        private bool _isSelected;

        public FileItemViewModel(LockerObject fileInfo)
        {
            _fileInfo = fileInfo;
            SelectCommand = new RelayCommand(p =>
            {
                IsSelected = !IsSelected;
                OnSelect?.Invoke(this, null);
            });

            DownloadCommand = new RelayCommand(p =>
            {
                DownloadFile();
            });
        }

        private async Task DownloadFile()
        {
            //TODO: show passwordbox

            var pwd = NavigationController.Instance.ShowView<EnterPasswordViewModel>("enter-passwd");

            var pw = await pwd.Task;

            NavigationController.Instance.ShowView<ExplorerViewModel>("home");
            NavigationController.Instance.DeleteView<EnterPasswordViewModel>("enter-passwd");

            var file = await BackendConnector.Instance.RequestFileDownload(_fileInfo, pw);

            var dialog = new SaveFileDialog();
            dialog.ValidateNames = true;
            dialog.Title = Resources.DownloadFile;
            dialog.AddExtension = true;
            dialog.DefaultExt = file.Extension;

            if (dialog.ShowDialog() == true)
            {
                var data = Convert.FromBase64String(file.Data);
                using (var stream = dialog.OpenFile())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
        }


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public Size Size
        {
            get { return Properties.Settings.Default.FileItemSize; }
        }

        public Bitmap Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
            }
        }
    }
}
