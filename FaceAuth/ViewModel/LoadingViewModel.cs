using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuth.ViewModel
{
    class LoadingViewModel : BaseViewModel
    {
        private string message;

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public override void Init()
        {
            WindowProperties.Height = 500;
            WindowProperties.Width = 400;
            WindowProperties.CanResize = false;
        }
    }
}
