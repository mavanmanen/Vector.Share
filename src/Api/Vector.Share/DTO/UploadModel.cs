using Microsoft.AspNetCore.Http;

namespace Vector.Share.DTO
{
    public class UploadModel
    {
        public IFormFile FileData { get; set; }
        public FileLifetime Lifetime { get; set; }
    }
}