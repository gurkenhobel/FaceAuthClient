using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FaceAuth.Annotations;

namespace FaceAuth.ViewModel
{
    /// <summary>
    /// Contains the Properties which will be bound to the Properties of the window
    /// </summary>
    class WindowProperties : INotifyPropertyChanged
    {
        /// <summary>
        /// part of the databinding. notifies the view about property changes
        /// </summary>
        #region PropertyChanged impl

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //Default Instance for the Designer
        public static WindowProperties Default = new WindowProperties() {CanResize = false, Height = 500, Width = 400, Title = "Default"};

        private int _width;
        private int _height;
        private string _title;
        private bool _canResize;

        public int Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public bool CanResize
        {
            get
            {
                return _canResize;
            }

            set
            {
                _canResize = value;
                OnPropertyChanged(nameof(CanResize));
            }
        }
    }
}
