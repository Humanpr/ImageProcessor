using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using processimage.BitmapHelper;
using Serilog;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace processimage.Filters.ConvolutionFilters
{
    /// <summary>
    /// Applies convolution filters by switching requested filter and applying matrix that corresponds to that filter.
    /// </summary>
    class ConvolutionFilterApplyer
    {
        FilterOptions filter;
        string filePath;
        
        public ConvolutionFilterApplyer(FilterOptions _filter, string _filePath)
        {
            filter = _filter;
            filePath = _filePath;
        }


        public void ApplyFilter(string outputFileName, int weight = 4)
        {
            //switching filter then sending right matrix to Conv3x3 method
            switch (filter)
            {
                case FilterOptions.Embose:
                    Log.Debug("FilePath: {path}", filePath);
                    ApplyConv3x3(outputFileName, StringToMatrix("{-1,0,-1} {0,4,0} {-1,0,-1}",127,1));
                    break;
                case FilterOptions.GaussianBlur:
                    Log.Debug("FilePath: {path}", filePath);
                    ApplyConv3x3(outputFileName, StringToMatrix("{1,2,1} {2,4,2} {1,2,1}", 0, 0));
                    break;

            }
        }
        /// <summary>
        /// Takes filter matrix and applies it to image bytes.
        /// </summary>
        /// <param name="outputFileName"></param>
        /// <param name="matrix"></param>
        public void ApplyConv3x3(string outputFileName, ConvMatrix matrix) {

            //if (matrix.Factor == 0) { return; }

            (int Stride, byte[] imageBytes, int Width, int Height,PixelFormat pixelFormat) = new BitmapDataExtractor(filePath);

            byte[] WorkBytes = new byte[Height * Stride];
            
            

            // configuring blue red green color

            int nPixel = 0;

            for (int h = 0; h < Height - 3; h++)
            {

                for (int c = 0; c < Stride; c += 1)
                {

                    // blue
                    nPixel = (((
                    (imageBytes[h * Stride + c] * matrix.TopLeft) + (imageBytes[h * Stride + c + 3] * matrix.TopMid) + (imageBytes[h * Stride + c + 6] * matrix.TopRight)
                    + (imageBytes[(h + 1) * Stride + c] * matrix.MidLeft) + (imageBytes[(h + 1) *Stride  + c + 3] * matrix.Pixel) + (imageBytes[(h + 1) * Stride + c + 6] * matrix.MidRight) +
                    (imageBytes[(h + 2) * Stride + c] * matrix.BottomLeft) + (imageBytes[(h + 2) * Stride + c + 3] * matrix.BottomMid) + (imageBytes[(h + 2) * Stride + c + 6] * matrix.BottomRight)) / matrix.Factor) + matrix.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) { nPixel = 255; }

                    WorkBytes[(h + 1) * Stride + c + 3] = (byte)nPixel;
                    // green
                    nPixel = (((
                    (imageBytes[h * Stride + c + 1] * matrix.TopLeft) + (imageBytes[h * Stride  + c + 4] * matrix.TopMid) + (imageBytes[h * Stride  + c + 7] * matrix.TopRight)
                    + (imageBytes[(h + 1) * Stride + c + 1] * matrix.MidLeft) + (imageBytes[(h + 1) * Stride + c + 4] * matrix.Pixel) + (imageBytes[(h + 1) * Stride + c + 7] * matrix.MidRight) +
                    (imageBytes[(h + 2) * Stride + c + 1] * matrix.BottomLeft) + (imageBytes[(h + 2) * Stride + c + 4] * matrix.BottomMid) + (imageBytes[(h + 2) * Stride + c + 7] * matrix.BottomRight)) / matrix.Factor) + matrix.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;

                    WorkBytes[(h + 1) * Stride + c + 4] = (byte)nPixel;
                    // red
                    nPixel = (((
                    (imageBytes[h * Stride  + c + 2] * matrix.TopLeft) + (imageBytes[h * Stride  + c + 5] * matrix.TopMid) + (imageBytes[h * Stride + c + 8] * matrix.TopRight)
                    + (imageBytes[(h + 1) * Stride + c + 2] * matrix.MidLeft) + (imageBytes[(h + 1) * Stride + c + 5] * matrix.Pixel) + (imageBytes[(h + 1) * Stride + c + 8] * matrix.MidRight) +
                    (imageBytes[(h + 2) * Stride + c + 2] * matrix.BottomLeft) + (imageBytes[(h + 2) * Stride + c + 5] * matrix.BottomMid) + (imageBytes[(h + 2) * Stride + c + 8] * matrix.BottomRight)) / matrix.Factor) + matrix.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;

                    WorkBytes[(h + 1) * Stride + c + 5] = (byte)nPixel;
                }

            }
            BitmapBsSaver.Save(WorkBytes, outputFileName, Width, Height,pixelFormat);

        }
        /// <summary>
        /// Takes matrixString,int offset and factor in {1,2,3} {4,5,6} format and return ConvolutionFilter.
        /// If factor is 0 then factor will be calculated by summing all components of matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="off"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public ConvMatrix StringToMatrix(string matrix, int off,int factor=0) // Format {1,2,3} {4,5,6} {7,8,9}
        {
            var list = matrix.Split(" ").Select(row => row.Trim('{', '}').Split(',').Select(x => Convert.ToInt32(x))).SelectMany(x => x).ToArray();
            
            var matrixObj = new ConvMatrix();

            matrixObj.TopLeft = list[0];
            matrixObj.TopMid = list[1];
            matrixObj.TopRight = list[2];

            matrixObj.MidLeft = list[3];
            matrixObj.Pixel = list[4];
            matrixObj.MidRight = list[5];

            matrixObj.BottomLeft = list[6];
            matrixObj.BottomMid = list[7];
            matrixObj.BottomRight = list[8];
            if (factor == 0)
            {
                matrixObj.Factor = list.Sum();
            }
            else {
                matrixObj.Factor = factor;
            }

            matrixObj.Offset = off;
            return matrixObj;
        }
    }
    /// <summary>
    /// Stores 3x3 matrix, factor and offset
    /// </summary>
    public class ConvMatrix
    {
        public int TopLeft = -1, TopMid =0, TopRight =-1;
        public int MidLeft = 0, Pixel =4, MidRight = 0;
        public int BottomLeft = -1, BottomMid = 0, BottomRight = -1;
        public int Factor = 1;
        public int Offset = 127;

        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }
    }
}
