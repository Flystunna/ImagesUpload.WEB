using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImagesUpload.WEB.Utils
{
    public class Constants
    {
        public const string ClientNoToken = "webapi_no_token";
        public const string ClientWithToken = "webapi_with_token";
        public const string HumanDateFormat = "dd MMMM, yyyy";
        public const string SystemDateFormat = "dd/MM/yyyy";
        public class Keys
        {
            public const string JwtBearerSection = "Authentication:JwtBearer";
            public const string ApiBaseUrl = "App:ApiBaseUrl";
            public const string Alerts = "_Alerts";
        }
        public class Routes
        {
            public const string PostImage = "/api/image/PostImage";
            public const string SearchImages = "/api/image/SearchImages";
            public const string GetAllImages = "/api/image/GetAllImages";
        }
    }
}
