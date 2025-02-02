using DioLabs.Netflix.DataAdd;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DioLabs.Netflix.CreateMovie;

public class CreateMovieFunction
{
    private readonly ILogger<CreateMovieFunction> _logger;

    public CreateMovieFunction(ILogger<CreateMovieFunction> logger)
    {
        _logger = logger;
    }

    [Function("CreateMovie")]
    [CosmosDBOutput("%DatabaseName%", "%ContainerName%", Connection = "CosmosDBConnection", CreateIfNotExists = true, PartitionKey = "id")]
    public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        
        var createMovieRequest = await req.ReadFromJsonAsync<CreateMovieRequest>();

        if (createMovieRequest == null)
        {
            return new BadRequestObjectResult("Dados inválidos.");
        }

        return JsonConvert.SerializeObject(createMovieRequest);
    }
}
