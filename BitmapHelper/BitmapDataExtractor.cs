using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Serilog;

namespace processimage.BitmapHelper
{
    /// <summary>
    /// Extracts bitmap stride,bytes,height,width. Thus we write less boilerplate code.
    /// Gets information form constructor and gives information via deconstructor.
    /// </summary>
    class BitmapDataExtractor
    {
        byte[] imageBytes;
        int Stride;
        PixelFormat pixelFormat;
        int Width, Height;
        //public bool isAlpha; // if target bitmap format contains alpha values
        public BitmapDataExtractor(string BmpPath)
        {
            Log.Debug("Received path",BmpPath);
            Bitmap bitmap = new Bitmap(BmpPath);
            //isAlpha = bitmap.PixelFormat.ToString().Contains("A"); // if format name contains 'A'=> we need to know it while applying filter
            Width = bitmap.Width;
            Height = bitmap.Height;
            pixelFormat=bitmap.PixelFormat;
            Log.Debug($"Bitmap file format is {pixelFormat}");
            
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,0,bitmap.Width,bitmap.Height),ImageLockMode.WriteOnly,bitmap.PixelFormat);
            Stride = bitmapData.Stride;
            imageBytes = new byte[Height * Stride];
            Marshal.Copy(bitmapData.Scan0,imageBytes,0,imageBytes.Length);
            bitmap.UnlockBits(bitmapData);
        }

        public void Deconstruct(out int _Stride,out byte[] _imageBytes,out int _Width,out int _Height,out PixelFormat _pixelFormat) {
            _Stride = Stride;
            _imageBytes = imageBytes;
            _Width = Width;
            _Height = Height;
            _pixelFormat=pixelFormat;
        }

    }
}
