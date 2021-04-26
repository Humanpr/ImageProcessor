using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using processimage.BitmapHelper;

namespace processimage.Filters
{
    /// <summary>
    /// Class that Applyes Grayscale Filter
    /// </summary>
    class GrayscaleFilter
    {
        string outFileName;
        string filePath;

        /// <summary>
        /// Takes filePath of image that filter applies on. And takes outFileName(path) where result image will be saved.
        /// </summary>
        /// <param name="_filePath"></param>
        /// <param name="_outFileName"></param>
        public GrayscaleFilter(string _filePath,string _outFileName)
        {
            outFileName = _outFileName;
            filePath = _filePath;
        }
        /// <summary>
        /// Processes image and saves new image
        /// </summary>
        public void Apply() {
            (int Stride, byte[] imageBytes,int Width,int Height) = new BitmapDataExtractor(filePath);
            byte[] OutputBytes = imageBytes;

            for (int i = 0; i < Height; i++)
            {
                for (int a = 0; a < Stride; a = a + 3)
                { 
                    MakePixelGray(ref imageBytes, i * Stride + a);
                }
            }
            BitmapBsSaver.Save(imageBytes,outFileName,Width,Height); // saves bitmap with specified byte array
        }
        /// <summary>
        /// Takes byte array ref and avareges first 3 pixel components and sets avarage value to that first 3 pixel components
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="indx"></param>
        void MakePixelGray(ref byte[] bytes, int indx)
        {
            var b = bytes[indx];
            var g = bytes[indx + 1];
            var r = bytes[indx + 2];
            var gray = (byte)((b + g + r) / 3);
            bytes[indx] = gray;
            bytes[indx + 1] = gray;
            bytes[indx + 2] = gray;
        }

    }
}
