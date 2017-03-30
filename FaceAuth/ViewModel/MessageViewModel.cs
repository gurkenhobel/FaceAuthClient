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
    class MessageViewModel:BaseViewModel
    {
        private string _message;
        private string _buttonMessage;
        private ICommand _messageActionCommand;


        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string ButtonMessage
        {
            get
            {
                return _buttonMessage;
            }

            set
            {
                _buttonMessage = value;
                OnPropertyChanged(nameof(ButtonMessage));
            }
        }

        public ICommand MessageActionCommand
        {
            get
            {
                return _messageActionCommand;
            }

            set
            {
                _messageActionCommand = value;
                OnPropertyChanged(nameof(MessageActionCommand));
            }
        }


       


        public override void Init()
        {
            WindowProperties.Height = 500;
            WindowProperties.Width = 400;
            WindowProperties.CanResize = false;
            WindowProperties.Title = Resources.AppName;
        }
    }
}
