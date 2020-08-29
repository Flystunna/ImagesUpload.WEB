using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImagesUpload.WEB.Models
{
    public class SearchImages
    {
        public long? FileSize { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
    }
}
