using azure_functions_dotnet_quickstart.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly: FunctionsStartup(typeof(azure_functions_dotnet_quickstart.Startup))]

namespace azure_functions_dotnet_quickstart
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            builder.ConfigurationBuilder
               .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.json"), true, true)
               .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{env}.json"), true, true);

            builder.ConfigurationBuilder.AddEnvironmentVariables();
            builder.ConfigurationBuilder.Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.Services.AddOptions()
                       .Configure<CosmosDbServiceOptions>(context.Configuration.GetSection("CosmosDb"));

            builder.Services
                .AddHttpClient()
                .AddSingleton<CosmosDbService>();
        }
    }
}
