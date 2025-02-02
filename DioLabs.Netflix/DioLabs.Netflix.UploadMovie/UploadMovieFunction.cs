using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DioLabs.Netflix.UploadMovie;

public class UploadMovieFunction
{
    private readonly ILogger<UploadMovieFunction> _logger;

    public UploadMovieFunction(ILogger<UploadMovieFunction> logger)
    {
        _logger = logger;
    }

    [Function("UploadMovie")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Processando a m�dia no storage");

        if (!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
        {
            return new BadRequestObjectResult("O header 'file-type' � obrigat�rio.");
        }

        var fileType = fileTypeHeader.ToString();
        var form = await req.ReadFormAsync();
        var file = form.Files["file"];

        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("O arquivo n�o foi enviado.");
        }

        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        string containerName = fileType;

        BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

        var blobName = file.FileName;
        var blob = containerClient.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            await blob.UploadAsync(stream, true);
        }

        _logger.LogInformation($"Arquivo {file.FileName} armazenado com sucesso.");

        return new OkObjectResult(new 
        { 
            Message = "Arquivo armazenado com sucesso.",
            BlobUri = blob.Uri
        });
    }
}
