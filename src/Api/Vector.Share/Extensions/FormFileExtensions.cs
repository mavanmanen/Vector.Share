using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Vector.Share.Extensions
{
    public static class FormFileExtensions
    {
        public static async Task<byte[]> GetFileDataAsync(this IFormFile file)
        {
            var buffer = new byte[file.Length];

            await using Stream fileStream = file.OpenReadStream();
            await fileStream.ReadAsync(buffer, 0, buffer.Length);

            return buffer;
        }
    }
}
