using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuth.Contract
{
    class FileCatalogRequest: Message
    {
        public FileCatalogRequest()
        {
            MessageType = MessageType.FileCatalogRequest;
        }
    }
}
