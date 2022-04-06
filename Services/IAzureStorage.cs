using System.IO;
using System.Threading.Tasks;

namespace Hire360WebAPI.Services
{
    public interface IAzureStorage
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
        Task<string> UploadAsyncProfilePicture(Stream fileStream, string fileName, string contentType);
    }
}