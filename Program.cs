using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace IEEEOUIparser
{
    public static class Program
    {
        public static IConfigurationRoot configuration;
        private static IServiceProvider serviceProvider;
        private static Startup startup;

        public static void Main()
        {
            try
            {
                // Initialize serilog logger
                Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                 .MinimumLevel.Debug()
                 .Enrich.FromLogContext()
                 .CreateLogger();

                configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                    .AddJsonFile("appsettings.json", false)
                    .Build();

                serviceProvider = RunSetup();
                IApplicationBuilder appBuilder = new ApplicationBuilder(serviceProvider);
                startup.Configure(appBuilder);

                LoadOui app = serviceProvider.GetService<LoadOui>();
                app.RunService();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IServiceProvider RunSetup()
        {
            IServiceCollection services = new ServiceCollection();
            startup = new Startup(configuration);
            startup.ConfigureServices(services);
            return services.BuildServiceProvider();
        }
    }
}