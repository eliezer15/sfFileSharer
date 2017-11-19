using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebhookEndpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var urls = environment == EnvironmentName.Development
                ? "http://localhost:5050"
                : "http://*:80";

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(urls)
                .UseSerilog((hostingContext, loggerConfig ) => loggerConfig
                    .WriteTo.File("log.txt"))
                .Build();
        }
    }
}
