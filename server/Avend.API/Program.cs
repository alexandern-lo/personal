using System;
using System.IO;

using Microsoft.AspNetCore.Hosting;

namespace Avend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();

            if ("1" == Environment.GetEnvironmentVariable("LOCAL"))
            {
                host = host.UseKestrel(options =>
                {
                    options.UseHttps("./certs/certificate.pfx");
                })
                .UseUrls("https://0.0.0.0:5000");
            }
            else if("0" == Environment.GetEnvironmentVariable("LOCAL"))
            {
                host = host.UseKestrel(options =>
                {
                    options.UseHttps("./certs/certificate.pfx");
                })
                .UseUrls("https://0.0.0.0:5000");
            }
            {
                host = host.UseKestrel();
            }

            host.Build().Run();
        }
    }
}
