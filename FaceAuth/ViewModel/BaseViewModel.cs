using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using FaceAuth.Annotations;

namespace FaceAuth.ViewModel
{
    class BaseViewModel: MarkupExtension, INotifyPropertyChanged
    {
        private WindowProperties _windowProperties;

        public WindowProperties WindowProperties
        {
            get
            {
                return _windowProperties;
            }

            set
            {
                _windowProperties = value;
                OnPropertyChanged(nameof(WindowProperties));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Init() {}

        public virtual void Stop() { }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
