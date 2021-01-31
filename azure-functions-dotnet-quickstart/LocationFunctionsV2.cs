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
using azure_functions_dotnet_quickstart.Services;

namespace azure_functions_dotnet_quickstart
{
    public class LocationFunctionsV2
    {
        private readonly HttpClient _httpClient;
        private readonly CosmosDbService _cosmosService;

        public LocationFunctionsV2(HttpClient httpClient, CosmosDbService cosmosService)
        {
            _httpClient = httpClient;
            _cosmosService = cosmosService;
        }

        [FunctionName("GetLocationForAddressV2")]
        public async Task<IActionResult> GetLocationForAddress(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var address = req.Query["addr"].ToString();
            var city = req.Query["city"].ToString();
            var id = req.Query["id"].ToString();

            var locationResponse = await _httpClient.GetStreamAsync($"https://geogratis.gc.ca/services/geolocation/en/locate?q={address}");
            var locationResult = await JsonSerializer.DeserializeAsync<IEnumerable<LocationResult>>(locationResponse);
            var bestMatch = locationResult.FirstOrDefault();
            if (bestMatch == null)
                return new NotFoundObjectResult($"Cannot find location for address '{address}'");

            await _cosmosService.UpdateItemLocation(id, address, city, bestMatch.geometry);

            return new OkObjectResult($"Location for address '{address}' is: [{bestMatch.geometry.coordinates[0]},{bestMatch.geometry.coordinates[1]}]. Database Updated!");
        }
    }
}
