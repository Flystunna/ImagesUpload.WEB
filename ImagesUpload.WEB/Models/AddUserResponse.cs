using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImagesUpload.WEB.Models
{
    public class AddUserResponseDTO 
    {
        public bool resp { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
    }
}
