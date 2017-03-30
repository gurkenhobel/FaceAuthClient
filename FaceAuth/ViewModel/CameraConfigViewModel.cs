using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AForge.Video.DirectShow;
using FaceAuth.Hardware;
using FaceAuth.Properties;
using FaceAuth.Util;

namespace FaceAuth.ViewModel
{
    class CameraConfigViewModel : BaseViewModel
    {

        private CameraController _cc;
        private ObservableCollection<FilterInfo> _devices;
        private ObservableCollection<VideoCapabilities> _resolutions;
        private FilterInfo _selectedDevice;
        private VideoCapabilities _selectedResolution;

        private CameraConfig _config;

        public Action ResultAction { get; set; }

        public ICommand OkCommand { get; private set; }

        public ObservableCollection<FilterInfo> Devices
        {
            get
            {
                return _devices;
            }

            set
            {
                _devices = value;
                OnPropertyChanged(nameof(Devices));
            }
        }

        public ObservableCollection<VideoCapabilities> Resolutions
        {
            get
            {
                return _resolutions;
            }

            set
            {
                _resolutions = value;
                OnPropertyChanged(nameof(Resolutions));
            }
        }

        public FilterInfo SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }

            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Resolutions = new ObservableCollection<VideoCapabilities>(_cc.GetResolutionsForDevice(value));
                    _config.SelectedDevice = value.MonikerString;
                });
            }
        }

        public VideoCapabilities SelectedResolution
        {
            get
            {
                return _selectedResolution;
            }

            set
            {
                _selectedResolution = value;
                OnPropertyChanged(nameof(SelectedResolution));

                _config.SelectedResolution = value.FrameSize;
            }
        }

        public CameraConfigViewModel()
        {
            OkCommand = new RelayCommand(p =>
            {
                CameraConfig.SaveConfig(_config);
                ResultAction.Invoke();
            });
           
        }


        public override void Init()
        {
            WindowProperties.Width = 400;
            WindowProperties.Height = 500;
            WindowProperties.CanResize = false;
            WindowProperties.Title = Resources.ConfigureCamera;
            _cc = new CameraController();
            Devices = new ObservableCollection<FilterInfo>(_cc.GetDevices());
            _config = new CameraConfig();
        }
    }
}
