using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Starwars.Core.Config
{
    public class CurrentConfiguration
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }



        public static CurrentConfiguration Build() {

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrEmpty(env))
            {
                env = "Development";
            }

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appSettings.{env}.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var config = new CurrentConfiguration()
            {
                BootstrapServers = configuration["bootstrapServers"],
                Topic = configuration["topic"]
            };

            return config;

        }
    }
}
