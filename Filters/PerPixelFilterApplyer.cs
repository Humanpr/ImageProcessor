using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace processimage.Filters
{
    class PerPixelFilterApplyer
    {
        FilterOptions filter;
        string filePath;
        

        public PerPixelFilterApplyer(FilterOptions _filter, string _filePath)
        {
            
            filter = _filter;
            filePath = _filePath;
           
        }

        public void ApplyFilter(string outputFileName, int level=50) {
            switch (filter) {
                case FilterOptions.Grayscale:
                    Log.Debug("FilePath: {path}",filePath);
                    new GrayscaleFilter(filePath,outputFileName).Apply();
                    break;
                case FilterOptions.Invert:
                    Log.Debug("FilePath: {path}", filePath);
                    new InvertFilter(filePath, outputFileName).Apply();
                    break;
                case FilterOptions.Brightness:
                    Log.Debug("FilePath: {path}", filePath);
                    new BrightnessFilter(filePath, outputFileName,level).Apply();
                    break;
            }
        }

    }
}
