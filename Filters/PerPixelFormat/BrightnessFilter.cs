using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using processimage.BitmapHelper;

namespace processimage.Filters
{
    /// <summary>
    /// Aplies Brightnessfilter takes level integer
    /// </summary>
    class BrightnessFilter
    {
        string outFileName;
        string filePath;
        int level;
        /// <summary>
        /// Takes filePath of image that filter applies on.And takes level of effect. And takes outFileName(path) where result image will be saved.
        /// </summary>
        /// <param name="_filePath"></param>
        /// <param name="_outFileName"></param>
        public BrightnessFilter(string _filePath, string _outFileName,int _level)
        {
            outFileName = _outFileName;
            filePath = _filePath;
            level = _level;
        }
        public void Apply()
        {
            (int Stride, byte[] imageBytes, int Width, int Height) = new BitmapDataExtractor(filePath);
            byte[] OutputBytes = imageBytes;

            for (int i = 0; i < Height; i++)
            {
                for (int a = 0; a < Stride; a = a + 1)
                {
                    var BrightenedPixel = imageBytes[i * Stride + a] + level;
                    if (BrightenedPixel > 255) { BrightenedPixel = 255; }
                    if (BrightenedPixel < 0) { BrightenedPixel = 0; }
                    imageBytes[i * Stride + a] = (byte)BrightenedPixel;
                }
            }
            BitmapBsSaver.Save(imageBytes, outFileName, Width, Height); // saves bitmap with specified byte array
        }
    }
}
