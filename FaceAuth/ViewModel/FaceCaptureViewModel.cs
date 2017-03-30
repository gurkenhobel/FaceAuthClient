using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FaceAuth.Hardware;
using FaceAuth.Util;

namespace FaceAuth.ViewModel
{
    class FaceCaptureViewModel : BaseViewModel
    {
        private ImageSource _webcamImage;
        private bool _windowAdjusted;
        private CameraController _cc;
        private Bitmap _result;

        public event EventHandler<Bitmap> OnSnapshot;




        public ICommand SnapshotCommand { get; private set; }
        public ImageSource WebcamImage
        {
            get { return _webcamImage; }
            private set
            {
                _webcamImage = value;
                OnPropertyChanged(nameof(WebcamImage));
            }
        }

        public Bitmap Result
        {
            get
            {
                return _result;
            }

            set
            {
                _result = value;
            }
        }

        public FaceCaptureViewModel()
        {
            SnapshotCommand = new RelayCommand(TakeSnapshot);
        }

        public override void Stop()
        {
            SnapshotCommand = null;
            _cc?.Stop();
        }

        public override void Init()
        {


            WindowProperties.Title = "Scan Face...";
            WindowProperties.CanResize = false;
            _windowAdjusted = false;

            var cfg = CameraConfig.LoadConfig();

            if (cfg == null)
            {
                ConfigureWebcam();
                return;
            }

            _cc = new CameraController();
            _cc.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals(nameof(_cc.ImageSource)))
                {
                    WebcamImage = _cc.ImageSource;
                    if (!_windowAdjusted && _cc.ImageSource != null)
                    {
                        var imageRatio = _cc.ImageSource.Width / _cc.ImageSource.Height;

                        if (_cc.ImageSource.Height > _cc.ImageSource.Width)
                        {
                            WindowProperties.Height = 500;
                            WindowProperties.Width = (int)(500 * imageRatio);
                        }
                        else
                        {
                            WindowProperties.Width = 400;
                            WindowProperties.Height = (int)(400 * imageRatio);
                        }
                        _windowAdjusted = true;
                    }
                }
            };

            _cc.Start(cfg);

        }

        private void ConfigureWebcam()
        {
            var config = NavigationController.Instance.ShowView<CameraConfigViewModel>("cam-config");
            config.ResultAction = () =>
            {
                NavigationController.Instance.DeleteView<CameraConfigViewModel>("cam-config");
                var newView = NavigationController.Instance.ShowView<FaceCaptureViewModel>("capture");
                newView.SnapshotCommand = SnapshotCommand;
            };
        }

        private void TakeSnapshot(object parameter)
        {
            Result = _cc.Image;
            //Result?.Save(Directory.GetCurrentDirectory() + "/snapshot.png", ImageFormat.Png);   //DEBUG
            OnSnapshot?.Invoke(this, Result);
        }
    }
}
