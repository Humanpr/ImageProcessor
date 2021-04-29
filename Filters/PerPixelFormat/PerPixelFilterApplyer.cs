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
    /// Gets filter then switchs it then calls proper Filter
    /// </summary>
    class PerPixelFilterApplyer
    {
        FilterOptions filter;
        string filePath;
        

        public PerPixelFilterApplyer(FilterOptions _filter, string _filePath)
        {
            
            filter = _filter; 
            filePath = _filePath;
           
        }

        public void ApplyFilter(string outputFileName, int level) {
            switch (filter) {
                case FilterOptions.Grayscale:
                    Log.Debug("FilePath: {path}",filePath);
                    PerBitApply(MakePixelGray,outputFileName);
                    break;
                case FilterOptions.Invert:
                    Log.Debug("FilePath: {path}", filePath);
                    PerBitApply(InvertPixel,outputFileName);
                    break;
                case FilterOptions.Brightness:
                    Log.Debug("FilePath: {path}", filePath);
                    PerBitApply(BrightenPixel,filePath);
                    break;
            }
        }

        public delegate void PerPixelApplyerFunc(ref byte[] imageBytes, int indx, int pixelcomponentcount, int imageBitSize, int level = 0);


        public void BrightenPixel(ref byte[] imageBytes, int indx, int PixelComponentCount, int imageBitSizeForPixel, int level)
        {
            int maxIntensity = (int)Math.Pow(2, imageBitSizeForPixel / PixelComponentCount);
            var BrightenedPixel = imageBytes[indx] + level;
            if (BrightenedPixel > maxIntensity) { BrightenedPixel = maxIntensity; }
            if (BrightenedPixel < 0) { BrightenedPixel = 0; }
            imageBytes[indx] = (byte)BrightenedPixel;
        }

        public void InvertPixel(ref byte[] imageBytes, int indx,int PixelComponentCount, int imageBitSizeForPixel, int level)
        {
            imageBytes[indx] = (byte)((int)Math.Pow(2, imageBitSizeForPixel / PixelComponentCount) - imageBytes[indx]);    /*TODO fix 256 for 32 bit image*/
        }

        /// <summary>
        /// Takes byte array ref and avareges first 3 pixel components and sets avarage value to that first 3 pixel components
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="indx"></param>
        void MakePixelGray(ref byte[] bytes, int indx,int PixelComponentCount,int imageBitSize,int level)
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

        
        /// <summary>
        /// Iterates array and calls specified function for each pixel component.
        /// </summary>
        /// <param name="applyerFunction"></param>
        /// <param name="outputFileName"></param>
        public void PerBitApply(PerPixelApplyerFunc applyerFunction,string outputFileName,int level=0) {

            BitmapDataExtractor bitmapDataExtractor = new BitmapDataExtractor(filePath);

            (int Stride, byte[] imageBytes, int Width, int Height, PixelFormat pixelFormat) = bitmapDataExtractor;

            byte[] OutputBytes = imageBytes;
            
            int PixelComponentCount = Stride / Width;
            
            int imageBitSizeForPixel = Int32.Parse(Regex.Match(pixelFormat.ToString(), @"\d+").Value);
            // if Alpha value exists then we have 4 pixel components otherwise we have 3
            Log.Debug($"Calculated image bpp size is {imageBitSizeForPixel}");

            Log.Debug($"Calculated pixel compomemt max value is {Math.Pow(2, imageBitSizeForPixel / PixelComponentCount)}");
            for (int i = 0; i < Height; i++)
            {
                for (int a = 0; a < Stride - PixelComponentCount; a = a + PixelComponentCount)
                {
                    applyerFunction(ref imageBytes, i * Stride + a,PixelComponentCount, imageBitSizeForPixel, level);
                }
            }

            BitmapBsSaver.Save(imageBytes, outputFileName, Width, Height, pixelFormat); // saves bitmap with specified byte array
        }

    }
}
