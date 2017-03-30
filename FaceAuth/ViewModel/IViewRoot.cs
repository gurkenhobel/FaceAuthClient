using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceAuth.Annotations;

namespace FaceAuth.ViewModel
{
    interface IViewRoot
    {
        BaseViewModel CurrentView { get; set; }
        WindowProperties WindowProperties { get; set; }
    }
}
