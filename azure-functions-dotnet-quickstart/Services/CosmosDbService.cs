using Microsoft.Azure.Cosmos;
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

        public CosmosDbService()
        {
            var database = Environment.GetEnvironmentVariable("CosmosDbDatabase", EnvironmentVariableTarget.Process).ToString();
            var container = Environment.GetEnvironmentVariable("CosmosDbContainer", EnvironmentVariableTarget.Process).ToString();
            var connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString", EnvironmentVariableTarget.Process).ToString();

            _cosmosClient = new CosmosClient(connectionString);
            _cosmosContainer = _cosmosClient.GetContainer(database, container);
        }

        public async Task UpdateItemLocation(string id, string address, string city, Geometry geo)
        {
            await _cosmosContainer.UpsertItemAsync(new { id, address, city, location = geo }, new PartitionKey(city));
        }
    }
}
