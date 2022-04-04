using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace Hire360WebAPI.Services
{
  public class AzureStorage : IAzureStorage
  {
    private readonly string _storageConnectionString;

    public AzureStorage(IConfiguration configuration)
    {
      _storageConnectionString = configuration.GetConnectionString("AzureStorage");
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
      var container = new BlobContainerClient(_storageConnectionString, "resume");
      var createResponse = await container.CreateIfNotExistsAsync();

      if (createResponse != null && createResponse.GetRawResponse().Status == 201)
        await container.SetAccessPolicyAsync(PublicAccessType.Blob);

      var blob = container.GetBlobClient(fileName);
      await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

      await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

      return blob.Uri.ToString();
    }
  }
}