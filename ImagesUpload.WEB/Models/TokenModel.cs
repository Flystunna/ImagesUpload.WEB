using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImagesUpload.WEB.Models
{
    public class TokenModel : ServResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int? Validity { get; set; }
        public DateTime Expires { get; set; }
    }
    public class ServResponse
    {
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
    }


    public class RefreshTokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
