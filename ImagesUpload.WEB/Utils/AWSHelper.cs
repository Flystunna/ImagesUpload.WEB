using Amazon;
using Amazon.S3;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace ImagesUpload.WEB.Utils
{
    public class UploadFileS3
    {
        private static IAmazonS3 s3Client;         
        public static async Task SendS3(string AWSProfileName, string filePath, string bucketName, string keyName)
        {
            var chain = new CredentialProfileStoreChain();
            AWSCredentials aWSCredentials;
            if (chain.TryGetAWSCredentials(AWSProfileName, out aWSCredentials))
            {
                RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
                s3Client = new AmazonS3Client(aWSCredentials, bucketRegion);
                await UploadFileAsync(filePath, bucketName, keyName);
            }      
        }

        private static async Task UploadFileAsync(string filePath, string bucketName, string keyName)
        {
            try
            {             
                var fileTransferUtility = new TransferUtility(s3Client);

                await fileTransferUtility.UploadAsync(filePath, bucketName);
                Log.Information("Upload 2 completed");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                Log.Error("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                Log.Error("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

        }
    }
}
#region comments
//To create credentials file
//public static void WriteProfile(string accesskey, string secretkey, string profilename)
//{
//    var options = new CredentialProfileOptions
//    {
//        AccessKey = accesskey,
//        SecretKey = secretkey             
//    };
//    var profile = new CredentialProfile(profilename, options);
//    var sharedfile = new SharedCredentialsFile();
//    sharedfile.RegisterProfile(profile);
//}   

// Option 1. Upload a file. The file name is used as the object key name.
//await fileTransferUtility.UploadAsync(filePath, bucketName);
//Console.WriteLine("Upload 1 completed");

// Option 2. Specify object key name explicitly.

// Option 3. Upload data from a type of System.IO.Stream.
//using (var fileToUpload =
//    new FileStream(filePath, FileMode.Open, FileAccess.Read))
//{
//    await fileTransferUtility.UploadAsync(fileToUpload,
//                               bucketName, keyName);
//}
//Console.WriteLine("Upload 3 completed");

//// Option 4. Specify advanced settings.
//var fileTransferUtilityRequest = new TransferUtilityUploadRequest
//{
//    BucketName = bucketName,
//    FilePath = filePath,
//    StorageClass = S3StorageClass.StandardInfrequentAccess,
//    PartSize = 6291456, // 6 MB.
//    Key = keyName,
//    CannedACL = S3CannedACL.PublicRead
//};
//fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
//fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

//await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
//Console.WriteLine("Upload 4 completed");
#endregion