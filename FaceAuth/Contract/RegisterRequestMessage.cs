using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceAuth.Model;

namespace FaceAuth.Contract
{
    class RegisterRequestMessage: Message
    {
        public User User { get; set; }

        public IEnumerable<string> Images { get; set; }
        public RegisterRequestMessage()
        {
            MessageType = MessageType.RegisterRequest;
        }
    }
}
