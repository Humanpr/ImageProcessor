using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;
namespace processimage
{
    //TODO implement this

    /// <summary>
    /// Generates Custom Help Text For Application argument and Options
    /// </summary>
    class HelpTextGenerator : IHelpTextGenerator
    {
        public void Generate(CommandLineApplication application, TextWriter output)
        {
            var arguments = application.Arguments;
            var options = application.Options;
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var argumentInfo in arguments) {
                output.WriteLine($"{argumentInfo.Name} => {argumentInfo.Description}");
            }
            foreach (var optionInfo in options)
            {
                output.WriteLine($"{optionInfo.ShortName} => {optionInfo.Description}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            
        }
    }
}
