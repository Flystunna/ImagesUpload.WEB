using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace ImagesUpload.WEB.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string FileType { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public long? FileSize { get; set; }
        public IFormFile Picture { get; set; }
        public List<IFormFile> Pictures { get; set; }    
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string Span { get; set; }
    }
}
