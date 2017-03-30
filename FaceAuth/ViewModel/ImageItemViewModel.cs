using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using FaceAuth.Annotations;
using FaceAuth.Util;

namespace FaceAuth.ViewModel
{
    class ImageItemViewModel
    {
        
        public event EventHandler OnImageRemove;

        public ICommand RemoveImageCommand { get; private set; }
       
        public ImageItemViewModel()
        {
            RemoveImageCommand = new RelayCommand(p =>
            {
                OnImageRemove?.Invoke(this, null);
            });
        }

        private ImageSource _imageSource;
        private Bitmap _image;

        private bool _isAddButton;

        public ImageSource ImageSource
        {
            get
            {
                return _imageSource;
            }

            set
            {
                _imageSource = value;
            }
        }

        public bool IsAddButton
        {
            get
            {
                return _isAddButton;
            }

            set
            {
                _isAddButton = value;
            }
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
