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
            public const string AWSAccessKeyId = "AWS:AWSAccessKeyId";
            public const string AWSSecretKey = "AWS:AWSSecretKey";
            public const string S3BucketName = "AWS:S3BucketName";
            public const string AWSProfileName = "AWS:AWSProfileName";
            public const string Alerts = "_Alerts";
        }
        public class URL
        {
            public const string LoginPath = "/Home/login";
            public const string AccessDeniedPath = "/Home/Error";
            public const string Default = "/Home/Index";
        }
        public class Routes
        {
            public const string PostImage = "/api/image/PostImage";
            public const string SearchImages = "/api/image/SearchImages";
            public const string GetAllImages = "/api/image/GetAllImages";


            public const string Token = "/api/account/auth";
            public const string RefreshToken = "/api/account/refreshtoken";
            public const string Register = "/api/account/AddUser";
            public const string AccountGetProfile = "/api/account/getprofile";
            public const string AccountGetClaims = "/api/account/GetCurrentUserClaims";



            public const string AccountUpdateProfile = "/api/account/updateprofile";
            public const string AccountChangePassword = "/api/account/changepassword";
            public const string AccountResetPassword = "/api/account/ForgotPassword";
            public const string AccountResetNewPassword = "/api/account/ResetPassword";
        }
    }
}
