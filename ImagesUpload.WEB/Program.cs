using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ImagesUpload.WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .WriteTo.File("C://Logs" + "/ImageUpload.WEB/Log-.txt",
                       outputTemplate: "{NewLine}{NewLine} TIME: {Timestamp:HH:mm:ss} {NewLine} TYPE:{Level} {NewLine} MESSAGE:{Message}{NewLine} EXCEPTION: {Exception}",
                       rollingInterval: RollingInterval.Day,
                       shared: true)
           .CreateLogger();
            try
            {
                var host = CreateHostBuilder(args).Build();               

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseIISIntegration().UseStartup<Startup>();
                });
    }
}
