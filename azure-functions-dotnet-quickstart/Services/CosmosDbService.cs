using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace azure_functions_dotnet_quickstart.Services
{
    public class CosmosDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _cosmosContainer;

        public CosmosDbService(IOptions<CosmosDbServiceOptions> options)
        {
            _cosmosClient = new CosmosClient(options.Value.ConnectionString);
            _cosmosContainer = _cosmosClient.GetContainer(options.Value.DatabaseId, options.Value.ContainerId);

            _cosmosClient.CreateDatabaseIfNotExistsAsync(options.Value.DatabaseId).GetAwaiter().GetResult();
            _cosmosContainer.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(options.Value.ContainerId, "/city")).GetAwaiter().GetResult();
        }

        public async Task UpdateItemLocation(string id, string address, string city, Geometry geo)
        {
            await _cosmosContainer.UpsertItemAsync(new { id, address, city, location = geo }, new PartitionKey(city));
        }
    }
}
