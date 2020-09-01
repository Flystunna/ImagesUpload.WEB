//Read me for Image Upload Frontened
This is an MVC Application called Image Upload. Built using C# programming language on .NET Core 3.1 runtime. 
It is capable of running on multiple platforms efficiently.
Model-View-Controller architecture. Built using fast razor pages.

.NET Core 3.1 is the latest version on the Core platform released by microsoft. 
If you have high performance and scalable system, then I would advise using .NET Core.

This application allows upload of images.
Allow searching of uploaded images either by description, file type or file size.
Searching is case senstive.
Search results are 20 results per page on the UI.
Column sorting implemented.
Authorization and Authentication was implemented. Users can login and register to use the application.

Uses Credentials File for the AWS services authentication
Uses Credentials file to create AmazonS3Client.

Note:Kindly specify below in appsettings on the frontend
"AWS": {
    "S3BucketName": Your BucketName,
    "AWSProfileName": Your Profile Name
  },

The Frontend applcation has Authentication
The Frontend applcation has Drag and drop upload support
The Frontend applcation has Multiple documents upload support
The Frontend applcation has UX/Styling. Use of javascript for aesthetics.
The Frontend applcation has the ability to retrieve or display the uploaded image
The Frontend applcation is performance efficient.
The Frontend applcation has browser support beyond Google Chrome’s latest version
The Frontend applcation is mobile-friendly UI
The Frontend applcation  has logging abilities supported with serilog.
The frontend must validate that there is at least one search parameter.
The Frontend applcation is a service-oriented application.
The frontend handles all API response codes gracefully with human-readable messages.
The application supports Google Chrome’s latest version.
The Frontend applcation allows column ordering or sorting
The Frontend applcation allows multiple filter searching 
The Frontend applcation allows Case sensitive search
The frontend applciation has logging abilities


On start of application there is a login page.

Default user is {
    "Username": "administrator@gmail.com",
    "Password": "Admin12345."
}

However users can register. There is a register link in the navbar and underneath the login button.


To build and run the application 
If Visual Studio is installed on the Server this application is cloned to.
Just open the solution file and run in release Mode.


To Launch the frontend application with through IIS Express using command prompt.
cd to Appliaction Root folder that contains solution file and this README file
Once in the directory
run the following



cd ImagesUpload.WEB
dotnet run
SET ASPNETCORE_ENVIRONMENT=Development                                                    
SET LAUNCHER_PATH=bin\Debug\netcoreapp3.1\ImagesUpload.WEB.exe
cd ../.vs/ImagesUpload.WEB/config
"C:\Program Files\IIS Express\iisexpress.exe" /config:"applicationhost.config" /site:"ImagesUpload.WEB" /apppool:"ImagesUpload.WEB AppPool




