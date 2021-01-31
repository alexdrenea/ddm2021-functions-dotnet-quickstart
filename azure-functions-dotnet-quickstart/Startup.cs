using azure_functions_dotnet_quickstart.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(azure_functions_dotnet_quickstart.Startup))]

namespace azure_functions_dotnet_quickstart
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddHttpClient()
                .AddSingleton<CosmosDbService>();
        }
    }
}
