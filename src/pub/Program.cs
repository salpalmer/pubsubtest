using Serilog;
using ILogger = Serilog.ILogger;

namespace pub
{
    class Program
    {
        private static string _appName = "";

        private static void Main(string[] args)
        {
            // Collect assembly information and throw Exception if we fail
            var assemblyName = typeof(Program).AssemblyQualifiedName?.Split(" ");
            if (assemblyName is null) throw new ApplicationException("Unable to fetch assembly information");

            _appName = assemblyName[1].Remove(assemblyName[1].IndexOf(assemblyName[1].Last()));

            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);
            try
            {
                Log.Information( "Configuring web host ({ApplicationContext})...", _appName );
                var host = CreateHostBuilder( args ).Build();
                Log.Information( "Starting web host ({ApplicationContext})...", _appName );
                host.Run();
            }
            catch ( Exception ex )
            {
                Log.Fatal( ex, "Program terminated unexpectedly ({ApplicationContext})!", _appName );
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog().ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(
                        options =>
                        {
                            options.Limits.MaxRequestBodySize = null;
                            options.Limits.MaxRequestBufferSize = null;
                            options.Limits.MaxResponseBufferSize = null;
                        });
                });

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationContext", _appName)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var appSettingsFileName =
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(appSettingsFileName, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}