using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using Serilog;

namespace processimage.BitmapHelper
{
    /// <summary>
    /// Contains Save method that gets imageBytes,fileName,Width,Height and saves.
    /// </summary>
    static class BitmapBsSaver
    {
        /// <summary>
        /// Takes imageBytes,outputFileName,Dimensions,Pixelformat and creates bitmap then saves image.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="fileName"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public static void Save(byte[] imageBytes, string fileName, int Width, int Height, PixelFormat pixelFormat)
        {
                Log.Debug($"ImageByte size is {imageBytes.Length}");
                Bitmap bitmap = new Bitmap(Width, Height);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, pixelFormat);
                // Locked bitmap to memory, so we can do operations on bitmap. It's more efficient than setPixel,getPixel.
                Marshal.Copy(imageBytes, 0, bitmapData.Scan0, imageBytes.Length); // Copying imagebytes to bitmap
                bitmap.UnlockBits(bitmapData); // Unlocking bitmap from the memory
                bitmap.Save(fileName);
        }
    }
}
