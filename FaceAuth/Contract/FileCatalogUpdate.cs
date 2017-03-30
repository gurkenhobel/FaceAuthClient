using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceAuth.Model;

namespace FaceAuth.Contract
{
    class FileCatalogUpdate: Message
    {
        public bool success { get; set; }
        public List<LockerObject> files { get; set; }
        public FileCatalogUpdate()
        {
            MessageType = MessageType.FileCatalogUpdate;
        }
    }
}
