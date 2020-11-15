using IEEEOUIparser.Abstract;
using IEEEOUIparser.MiddleWare;
using IEEEOUIparser.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace IEEEOUIparser
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure your services here

            services.AddSingleton<ILiteDbContext, NetworkContext>();
            services.AddTransient<NetworkContext>();
            services.AddTransient(x => new LoadOui(
                x.GetRequiredService<NetworkContext>(),
                x.GetRequiredService<IMacVenderLookup>(),
                x.GetRequiredService<IOptions<SettingOptions>>()
            ));

            services.AddSingleton(LoggerFactory.Create(b => b.AddSerilog(dispose: true)));
            services.AddLogging();

            // Build configuration
            //configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            //    .AddJsonFile("appsettings.json", false)
            //    .Build();

            // Add access to generic IConfigurationRoot
            services.AddSingleton(Configuration);
            services.AddTransient<IMacVenderLookup, MacVenderLookup>();
            services.Configure<LiteDbOptions>(Configuration.GetSection("LiteDbOptions"));
            services.Configure<RegExOptions>(Configuration.GetSection("RegEx"));
            services.Configure<SettingOptions>(Configuration.GetSection("Settings"));
        }

        public void Configure(IApplicationBuilder app)
            => app.UseMiddleware(typeof(ErrorHandlingMiddleware));
    }
}
