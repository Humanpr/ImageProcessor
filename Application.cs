using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Sinks.File;
using processimage.Filters;
using processimage.Filters.ConvolutionFilters;
using System.IO;
namespace processimage
{
   

    /// <summary>
    /// Handles option and argument user interface.
    /// </summary>
    class Application
    {
        public FilterOptions filter;
        public string outputFileName;
        public string ImagePath;

        
        public Application() {
            
        }
        
        public void StartAppLication(string[] args) {
                   
            CommandLineApplication application = new CommandLineApplication(); // aplication => processimage
            application.HelpOption();

            CommandLineApplication applyFilter = new CommandLineApplication(); // subcommand => applyfilter
            application.AddSubcommand(applyFilter);
            
            
            applyFilter.Name = "applyfilter";
            applyFilter.HelpOption("-h|--help");

            HelpTextGenerator helpTextGenerator = new HelpTextGenerator();

            //TODO Add custom help text generator?

            //applyFilter.UsePagerForHelpText = false;
            //applyFilter.HelpTextGenerator = (helpTextGenerator);

            var ImagePathOption = applyFilter.Option("-p|--path", "Path of image that will be processed.", CommandOptionType.SingleValue);
            ImagePathOption.IsRequired(false, "You should provide valid image path! Use -p or --path.");
            ImagePathOption.Accepts(x => x.ExistingFile("Specified file does not exist!"));

            var FilterOption = applyFilter.Option("-f|--filter", "Filter that will be applied to specified image ", CommandOptionType.SingleValue);
            FilterOption.IsRequired(false, "You need to specify valid filter try --help for filters");

            var OutPutFileNameArgument = applyFilter.Argument("Output file name", "Name of the file that will be outed as a result of processing");
            OutPutFileNameArgument.IsRequired(false, "No Empty name for output file name ");

            var LevelOfFilter = applyFilter.Option("-l|--level","Level of filter",CommandOptionType.SingleValue);
            

            applyFilter.OnExecute(() => {

                ImagePath = ImagePathOption.Value();
                
                string FilterName = FilterOption.Value();
                outputFileName = OutPutFileNameArgument.Value;

                filter=FilterName switch {
                    "Grayscale"=>FilterOptions.Grayscale,
                    "Brightness" => FilterOptions.Brightness,
                    "Contrast" => FilterOptions.Contrast,
                    "Embose" => FilterOptions.Embose,
                    "GaussianBlur" => FilterOptions.GaussianBlur,
                    "Invert" => FilterOptions.Invert,
                    "Sharpen" => FilterOptions.Sharpen,
                    "EdgeDedection" => FilterOptions.EdgeDedection,
                    "MeanBlur" => FilterOptions.MeanBlur,
                    _=>throw new Exception("Invalid Filter Name")
                };
                Log.Debug("User Input: Filter:{FilterName} ImagePath:{ImagePath} OutputFileName:{OutputFileName}",FilterName,ImagePath,outputFileName);
                var fileInfoSrcImg = new FileInfo(ImagePath);
                var fileInfoOutImg = new FileInfo(outputFileName);
                switch (filter) {
                    case FilterOptions.Brightness:
                    case FilterOptions.Grayscale:
                    case FilterOptions.Invert: //TODO make non level required ilters to not accept level option
                        new PerPixelFilterApplyer(filter,fileInfoSrcImg.DirectoryName+$"\\{fileInfoSrcImg.Name}").ApplyFilter(fileInfoOutImg.DirectoryName+$"\\{fileInfoOutImg.Name}",Int32.Parse(LevelOfFilter.Value()==null?"0":LevelOfFilter.Value()));
                        break;
                    case FilterOptions.GaussianBlur:
                    case FilterOptions.Embose:
                    case FilterOptions.Sharpen:
                    case FilterOptions.MeanBlur:
                        new ConvolutionFilterApplyer(filter, fileInfoSrcImg.DirectoryName + $"\\{fileInfoSrcImg.Name}").ApplyFilter(fileInfoOutImg.DirectoryName + $"\\{fileInfoOutImg.Name}");
                        break;
                }
                
            });

            Log.Debug("App started.. {path}",Directory.GetCurrentDirectory());
            application.Execute(args);
        }

    }
}
