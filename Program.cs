using IEEEOUIparser.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace IEEEOUIparser
{
    public static class Program
    {
        public static IConfigurationRoot configuration;

        public static void Main(string[] args)
        {
            // Initialize serilog logger
            Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                 .MinimumLevel.Debug()
                 .Enrich.FromLogContext()
                 .CreateLogger();

            var services = new ServiceCollection();
            ConfigureServices(services);

            try
            {
                using ServiceProvider serviceProvider = services.BuildServiceProvider();
                var LiteDbOptions = configuration.GetSection("LiteDbOptions").GetChildren().FirstOrDefault();
                LoadOui app = serviceProvider.GetService<LoadOui>();
                app.RunService();
            } catch(Exception ex)
            {
                Log.Fatal(ex.Message, ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<ILiteDbContext, NetworkContext>();
            services.AddTransient<NetworkContext>();
            services.AddTransient(x => new LoadOui(x.GetRequiredService<NetworkContext>()));
            services.AddSingleton(LoggerFactory.Create(builder => builder.AddSerilog(dispose: true)));
            services.AddLogging();

            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            services.AddSingleton(configuration);

            services.Configure<LiteDbOptions>(configuration.GetSection("LiteDbOptions"));
        }
    }
}