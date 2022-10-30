namespace Freyja;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Freyja.Configuration;

public class Program
{
    private static Features features = new();

    public static async Task Main(string[] args)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args);

        hostBuilder.ConfigureServices((context, services) =>
        {
            services.Configure<Features>(context.Configuration.GetSection(nameof(Features)));
            features = services.BuildServiceProvider().GetService<IOptions<Features>>()?.Value ?? new();
        });

        if (features.WebApi || features.WebUi)
        {
            hostBuilder.ConfigureWebHostDefaults(webHostBuilder =>
            {
                if (features.WebUi)
                {
                    webHostBuilder.Configure((context, app) =>
                    {
                        app.UseStaticFiles();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapFallbackToFile("index.html");
                        });
                    });
                }
            });
        }

        await hostBuilder.Build().RunAsync();
    }
}


