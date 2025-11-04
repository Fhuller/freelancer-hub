using System.IO;
using System.Threading.Tasks;

namespace freelancer_hub_backend.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName);
        Task<Stream> DownloadFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
    }
}
