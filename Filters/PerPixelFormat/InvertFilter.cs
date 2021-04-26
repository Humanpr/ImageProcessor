using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using processimage.BitmapHelper;

namespace processimage.Filters
{
    /// <summary>
    /// Invet filter
    /// </summary>
    class InvertFilter
    {
        string outFileName;
        string filePath;


        /// <summary>
        /// Takes filePath of image that filter applies on. And takes outFileName(path) where result image will be saved.
        /// </summary>
        /// <param name="_filePath"></param>
        /// <param name="_outFileName"></param>
        public InvertFilter(string _filePath, string _outFileName)
        {
            outFileName = _outFileName;
            filePath = _filePath;
        }

        public void Apply()
        {
            (int Stride, byte[] imageBytes, int Width, int Height) = new BitmapDataExtractor(filePath);
            byte[] OutputBytes = imageBytes;

            for (int i = 0; i < Height; i++)
            {
                for (int a = 0; a < Stride; a = a + 1)
                {
                    imageBytes[i * Stride + a] = (byte)(255 - imageBytes[i * Stride + a]);
                }
            }
            BitmapBsSaver.Save(imageBytes, outFileName, Width, Height); // saves bitmap with specified byte array
        }
    }
}
