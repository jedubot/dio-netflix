using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DioLabs.Netflix.GetMovieDetails
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

        [Function("GetMovieDetails")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var container = _cosmosClient.GetContainer("DioLabsNetflix", "movies");
            var id = req.Query["id"];
            var query = "SELECT * FROM c WHERE c.id = @id";
            var queryDefinition = new QueryDefinition(query).WithParameter("@id", id);
            var iterator = container.GetItemQueryIterator<MovieDetailsResponse>(queryDefinition);
            var results = new List<MovieDetailsResponse>();

            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }

            var responseMessage = req.CreateResponse(HttpStatusCode.OK);
            await responseMessage.WriteAsJsonAsync(results.FirstOrDefault());

            return responseMessage;
        }
    }
}
