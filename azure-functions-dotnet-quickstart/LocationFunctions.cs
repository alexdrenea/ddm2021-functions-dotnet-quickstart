using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;

namespace azure_functions_dotnet_quickstart
{
    public static class LocationFunctions
    {
        public static Lazy<HttpClient> HttpClient = new Lazy<HttpClient>(new HttpClient());

        public static Lazy<CosmosClient> CosmosClient = new Lazy<CosmosClient>(() =>
        {
            var connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString", EnvironmentVariableTarget.Process).ToString();

            return new CosmosClient(connectionString);
        });

        [FunctionName("GetLocationForAddress")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var address = req.Query["addr"].ToString();
            var city = req.Query["city"].ToString();
            var id = req.Query["id"].ToString();

            var locationResponse = await HttpClient.Value.GetStreamAsync($"https://geogratis.gc.ca/services/geolocation/en/locate?q={address}");
            var locationResult = await JsonSerializer.DeserializeAsync<IEnumerable<LocationResult>>(locationResponse);
            var bestMatch = locationResult.FirstOrDefault();
            if (bestMatch == null)
                return new NotFoundObjectResult($"Cannot find location for address '{address}'");

            var database = Environment.GetEnvironmentVariable("CosmosDbDatabase", EnvironmentVariableTarget.Process).ToString();
            var container = Environment.GetEnvironmentVariable("CosmosDbContainer", EnvironmentVariableTarget.Process).ToString();

            var cosmosContainer = CosmosClient.Value.GetContainer(database, container);
            var upsertResult = await cosmosContainer.UpsertItemAsync(new { id, address, city, location = bestMatch.geometry }, new PartitionKey(city));
        
            return new OkObjectResult($"Location for address '{address}' is: [{bestMatch.geometry.coordinates[0]},{bestMatch.geometry.coordinates[1]}]. Database Updated!");
        }
    }

    public class LocationResult
    {
        public string title { get; set; }
        public string qualifier { get; set; }
        public string type { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public float[] coordinates { get; set; }
    }

}
