using System.IO;
using System.Text;
using Serilog;

namespace PdfFlipBook.Helper.Logger
{
    public class Logger
    {
        public static Logger Instance = new Logger();

        public Serilog.Core.Logger _logger;
        private string _log = "Log.txt";

        private Logger()
        {
            if (!File.Exists(_log))
                File.Create(_log).Dispose();

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(_log, rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 5000000,
                    retainedFileCountLimit: 100,
                    encoding: Encoding.UTF8)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}