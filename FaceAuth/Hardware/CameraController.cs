using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using AForge.Video;
using AForge.Video.DirectShow;
using FaceAuth.Annotations;
using FaceAuth.Util;
using Size = System.Drawing.Size;

namespace FaceAuth.Hardware
{
    class CameraController : INotifyPropertyChanged
    {

        #region PropertyChanged impl
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        private Task _converterTask;
        private object _imageLocker = new object();

        public Bitmap Image
        {
            get
            {
                if (_image == null)
                    return null;
                lock (_imageLocker)
                    return (Bitmap)_image?.Clone();
            }
            private set
            {
                lock (_imageLocker)
                    _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                return _imageSource;
            }

            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        private VideoCaptureDevice _device;
        private Bitmap _image;
        private ImageSource _imageSource;


        public void Stop()
        {
            _device.NewFrame -= NewFrame;
            _device.Stop();
        }

        public void Start(CameraConfig cfg)
        {
            _converterTask = Task.Run(() => { });

            _device = new VideoCaptureDevice(cfg.SelectedDevice);
            if (cfg.SelectedResolution != Size.Empty)
                _device.VideoResolution = _device.VideoCapabilities.First(r => r.FrameSize == cfg.SelectedResolution);

            _device.NewFrame += NewFrame;

            _device.Start();
        }

        public List<FilterInfo> GetDevices()
        {
            List<FilterInfo> result = new List<FilterInfo>();
            FilterInfoCollection videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videosources.Count <= 0)
                throw new Exception("no webcam");
            foreach (var d in videosources)
            {
                result.Add((FilterInfo)d);
            }
            return result;
        }

        public List<VideoCapabilities> GetResolutionsForDevice(FilterInfo deviceInfo)
        {
            var device = new VideoCaptureDevice(deviceInfo.MonikerString);
            var result = device.VideoCapabilities.ToList();
            return result;
        }


        private void NewFrame(object s, NewFrameEventArgs e)
        {


            if (e.Frame == null)
                return;

            e.Frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
            Image = (Bitmap)e.Frame.Clone();

            if (_converterTask != null && _converterTask.IsCompleted)
            {
                var img = new Bitmap(e.Frame);
                _converterTask = Application.Current.Dispatcher.InvokeAsync(() =>
                {

                    ImageSource = ImageUtil.ImageSourceFromBitmap(img);
                    
                }).Task;
                _converterTask?.ContinueWith(t =>  img.Dispose());
            }

        }

    }
}
