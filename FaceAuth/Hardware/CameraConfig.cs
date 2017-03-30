using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FaceAuth.Hardware
{
    class CameraConfig
    {
        public string SelectedDevice { get; set; }
        public Size SelectedResolution { get; set; }

        #region fileio
        public static CameraConfig LoadConfig()
        {
            try
            {
                CameraConfig result = null;
                var json = File.ReadAllText(Directory.GetCurrentDirectory() + "/cfg/camera.cfg");
                result = JsonConvert.DeserializeObject<CameraConfig>(json);
                return result;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public static void SaveConfig(CameraConfig cfg)
        {
            var json = JsonConvert.SerializeObject(cfg);

            var cfgDir = Directory.GetCurrentDirectory() + "/cfg";
            if (!Directory.Exists(cfgDir))
            {
                Directory.CreateDirectory(cfgDir);
            }

            File.WriteAllText(cfgDir + "/camera.cfg", json);
        }
        #endregion


    }
}
