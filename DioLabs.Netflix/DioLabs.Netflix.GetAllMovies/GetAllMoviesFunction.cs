using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DioLabs.Netflix.GetAllMovies
{
    public class GetMovieDetailsFunction
    {
        private readonly CosmosClient _cosmosClient;
        private readonly ILogger<GetMovieDetailsFunction> _logger;

        public GetMovieDetailsFunction(CosmosClient cosmosClient, ILogger<GetMovieDetailsFunction> logger)
        {
            _cosmosClient = cosmosClient;
            _logger = logger;
        }

        [Function("GetAllMovies")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var container = _cosmosClient.GetContainer("DioLabsNetflix", "movies");
            var id = req.Query["id"];
            var query = "SELECT * FROM c";
            var queryDefinition = new QueryDefinition(query);
            var iterator = container.GetItemQueryIterator<GetAllMoviesResponse>(queryDefinition);
            var results = new List<GetAllMoviesResponse>();

            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }

            var responseMessage = req.CreateResponse(HttpStatusCode.OK);
            await responseMessage.WriteAsJsonAsync(results);

            return responseMessage;
        }
    }
}
