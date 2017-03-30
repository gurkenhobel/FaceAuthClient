using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FaceAuth.Util
{
    class ImageUtil
    {
       

        /// <summary>
        /// Converts System.Drawing.Bitmap to System.Windows.Media.imageSource
        /// </summary>
        /// <param name="bitmap">the bitmap to convert</param>
        /// <returns></returns>
        public static ImageSource ImageSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static byte[] BytesFromBitmap(Bitmap bitmap)
        {
            lock (bitmap)
            {
                if (bitmap == null)
                    return null;

                using (var memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    return memory.ToArray();
                }
            }
        }

        public static string Base64FromBitmap(Bitmap img)
        {
            var data = BytesFromBitmap(img);
            return Convert.ToBase64String(data);
        }
    }
}
