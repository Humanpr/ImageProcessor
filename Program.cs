using System;
using McMaster.Extensions.CommandLineUtils;
using Serilog;

namespace processimage
{

    // processimage -p image.png --filter grayscale grayscaleImage.png

    class Program
    {
        static void Main(string[] args)
        {
            // Created logger and setted to global static class
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("log.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .CreateLogger();

            // Initialising Application and passing args to start application
            Application app = new Application();
            try
            {
                app.StartAppLication(args);
            }
            catch (UnrecognizedCommandParsingException ex) {
                Log.Error(ex.Message);
                Error(ex.Message);
            }
            Log.CloseAndFlush();
        }


        public static void Error(string txt) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(txt);
            Console.ForegroundColor = ConsoleColor.White;
        
        }
    }
}
