using System;
using System.ComponentModel.DataAnnotations;
using Vector.Share.DTO;

namespace Vector.Share.Data.Models
{
    public class UploadedFile
    {
        [Key]
        public string Identifier { get; set; }
        public string Path { get; set; }
        public FileLifetime Lifetime { get; set; }
        public DateTime Uploaded { get; set; }
        public string ContentType { get; set; }
        public string OriginalFilename { get; set; }
    }
}
