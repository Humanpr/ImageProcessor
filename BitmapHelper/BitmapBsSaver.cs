using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace processimage.BitmapHelper
{
    /// <summary>
    /// Contains Save method that gets imageBytes,fileName,Width,Height and saves.
    /// </summary>
    static class BitmapBsSaver
    {
        /// <summary>
        /// Takes byte array fileName that final image will be saved and Dimensions. Then saves image.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="fileName"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public static void  Save(byte[] imageBytes, string fileName,int Width,int Height,PixelFormat pixelFormat) {
            Bitmap bitmap = new Bitmap(Width,Height);
            BitmapData bd = bitmap.LockBits(new Rectangle(0,0,Width,Height),ImageLockMode.WriteOnly,pixelFormat);
            Marshal.Copy(imageBytes,0,bd.Scan0,imageBytes.Length);
            bitmap.UnlockBits(bd);
            bitmap.Save(fileName);
        }
    }
}
