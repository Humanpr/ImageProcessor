using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using processimage.BitmapHelper;
using Serilog;

namespace processimage.Filters
{
    /// <summary>
    /// Gets FilterOption and applyes specified filter to image.
    /// </summary>
    class PerPixelFilterApplyer
    {
        FilterOptions filter;
        string srcImagePath;
        

        public PerPixelFilterApplyer(FilterOptions _filter, string _srcImagePath)
        {
            filter = _filter;
            srcImagePath = _srcImagePath;      
        }

        public void ApplyFilter(string outImageFilePath, int level) {
            switch (filter) {
                case FilterOptions.Grayscale:
                    Log.Debug("FilePath: {path}",srcImagePath);
                    PerBitApply(MakePixelGray,outImageFilePath);
                    break;
                case FilterOptions.Invert:
                    Log.Debug("FilePath: {path}", srcImagePath);
                    PerBitApply(InvertPixel,outImageFilePath);
                    break;
                case FilterOptions.Brightness:
                    Log.Debug("FilePath: {path}", srcImagePath);
                    PerBitApply(BrightenPixel,outImageFilePath,level);
                    break;
            }
        }

        // Delegate that applies logic on imageBytes
        public delegate void PerPixelApplyerFunc(ref byte[] imageBytes, int indx, int pixelcomponentcount, int imageBpp, int level = 0);
        
        /// <summary>
        /// Iterates array and calls specified function for each pixel component.
        /// </summary>
        /// <param name="applyerFunction"></param>
        /// <param name="outputFileName"></param>
        public void PerBitApply(PerPixelApplyerFunc applyerFunction, string outputFileName, int level = 0)
        {

            BitmapDataExtractor bitmapDataExtractor = new BitmapDataExtractor(srcImagePath);

            (int Stride, byte[] imageBytes, int Width, int Height, PixelFormat pixelFormat) = bitmapDataExtractor;

            //Extract data from bitmap

            int PixelComponentCount = Stride / Width;

            int imageBpp = Int32.Parse(Regex.Match(pixelFormat.ToString(), @"\d+").Value);
            // if Alpha value exists then we have 4 pixel components otherwise we have 3

            Log.Debug($"Calculated image bpp size is {imageBpp}");
            Log.Debug($"Calculated pixel compomemt max value is {Math.Pow(2, imageBpp / PixelComponentCount)}");

            // Iterating imageBytes by PixelComponentSize
            for (int i = 0; i < Height; i++)
            {
                for (int a = 0; a < Stride - PixelComponentCount; a = a + PixelComponentCount)
                {
                    applyerFunction(ref imageBytes, i * Stride + a, PixelComponentCount, imageBpp, level);
                }
            }

            BitmapBsSaver.Save(imageBytes, outputFileName, Width, Height, pixelFormat); // Saves bitmap with specified byte array
        }

        /// <summary>
        /// Takes PerPixelApplyerFunc arguments and brightens pixel.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="indx"></param>
        /// <param name="PixelComponentCount"></param>
        /// <param name="imageBpp"></param>
        /// <param name="level"></param>
        public void BrightenPixel(ref byte[] imageBytes, int indx, int PixelComponentCount, int imageBpp, int level)
        {
            int maxIntensity = (int)Math.Pow(2, imageBpp / PixelComponentCount)-1;

            var b = imageBytes[indx];
            var g = imageBytes[indx + 1];
            var r = imageBytes[indx + 2];

             imageBytes[indx] = (b+level) switch {var c when c< 0 => 0, var c when c > maxIntensity => (byte)(maxIntensity), var c =>(byte)c }  ;

             imageBytes[indx + 1] =  (g + level) switch { var c when c < 0 => 0, var c when c > maxIntensity => (byte)(maxIntensity), var c => (byte)c };

             imageBytes[indx + 2] =  (r + level) switch { var c when c < 0 => 0, var c when c > maxIntensity => (byte)(maxIntensity), var c => (byte)c };
            
             }


        /// <summary>
        /// Takes PerPixelApplyerFunc arguments and inverts pixel.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="indx"></param>
        /// <param name="PixelComponentCount"></param>
        /// <param name="imageBpp"></param>
        /// <param name="level"></param>
        public void InvertPixel(ref byte[] imageBytes, int indx,int PixelComponentCount, int imageBpp, int level)
        {
            int maxIntensty = (int)Math.Pow(2, imageBpp / PixelComponentCount)-1;
            
            imageBytes[indx] = (byte)(maxIntensty - imageBytes[indx]);    /*TODO fix 256 for 32 bit image*/

            imageBytes[indx + 1] = (byte)(maxIntensty - imageBytes[indx+1]);    

            imageBytes[indx + 2] = (byte)(maxIntensty - imageBytes[indx+2]);    
        }

        /// <summary>
        /// Takes PerPixelApplyerFunc delegate arguments and makes pixel gray (avarages bgr values and sets avarage to bgr again).
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="indx"></param>
        void MakePixelGray(ref byte[] bytes, int indx,int PixelComponentCount,int imageBpp,int level)
        {
            var b = bytes[indx];
            var g = bytes[indx + 1];
            var r = bytes[indx + 2];
            var a = PixelComponentCount == 4 ? bytes[indx + 3] : 0;

            var gray = (byte)((b + g + r + a) / PixelComponentCount);

            bytes[indx] = gray;
            bytes[indx + 1] = gray;
            bytes[indx + 2] = gray;

            if (PixelComponentCount == 4)
                bytes[indx + 3] = gray;
        }

        
        

    }
}
