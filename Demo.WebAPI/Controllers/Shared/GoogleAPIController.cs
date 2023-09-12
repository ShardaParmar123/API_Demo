using Demo.Contract.Services.Auth;
using Demo.Contract.Services.Shared;
using Demo.Types.Dtos.Auth;
using Demo.Types.Dtos.Shared;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Demo.WebAPI.Controllers.Shared
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleAPIController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly IHostEnvironment _environment;
        public GoogleAPIController(ILoggerService logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("UploadFile")]
        [AllowAnonymous]
        [Authorize]
        public async Task<ServiceResponse<Resource>> UploadFile(IFormFile file)
        {
            var sr = new ServiceResponse<Resource>();
            try
            {
                if (file == null || Path.GetExtension(file.FileName) != ".xlsx")
                {
                    sr.Success = false;
                    sr.Message = "Invalid or missing file. Please select xlsx file.";
                    return sr;
                }
                else
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string rootPath = _environment.ContentRootPath;

                    string[] scopes = new string[] { DriveService.Scope.Drive,
                               DriveService.Scope.DriveFile,};

                    //https://console.cloud.google.com/apis/credentials/oauthclient/655830380025-t9ulg7itj771grnpb1hgeknm5jr8jjr5.apps.googleusercontent.com?project=key-acronym-395510&supportedpurview=project

                    var clientId = "655830380025-t9ulg7itj771grnpb1hgeknm5jr8jjr5.apps.googleusercontent.com"; 
                    var clientSecret = "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";
                    var Refreshtoken = "1//04Ci8L1NJcTEJCgYIARAAGAQSNwF-L9IrWIg96X0pRDqCj2D1L4fS0bBljXNUizDgZHNlVQUZ5RA8iwNGtOxVWLQj-wWn10AStsQ";

                    var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    }, scopes,
                    Environment.UserName, CancellationToken.None, new FileDataStore(Refreshtoken)).Result;
                    //Once consent is recieved, your token will be stored locally on the AppData directory, so that next time you wont be prompted for consent.   

                    DriveService service = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "DemoAPI",

                    });
                    service.HttpClient.Timeout = TimeSpan.FromMinutes(100);                  
                    var respocne = uploadFile(service, fileName, "");
                    // Third parameter is empty it means it would upload to root directory, if you want to upload under a folder, pass folder's id here.
                    //MessageBox.Show("Process completed--- Response--" + respocne);
                    sr.Message = $"Process completed--- Response--\" + {respocne}";
                }
            }
            catch (Exception ex)
            {
                sr.Code = 0;
                sr.Message = $"{ex.Message}, File not uploaded in google drive, something went wrong.";
                _logger.LogError(sr.Message);
            }
            return sr;
        }       
        private Google.Apis.Drive.v3.Data.File uploadFile(DriveService _service, string _uploadFile, string _parent, string _descrp = "Uploaded with .NET!")
        {
            if (System.IO.File.Exists(_uploadFile))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = System.IO.Path.GetFileName(_uploadFile);
                body.Description = _descrp;
                //body.MimeType = GetMimeType(_uploadFile);
                body.MimeType = GetMimeTypeForFileExtension(_uploadFile);

                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = _service.Files.Create(body, stream, GetMimeTypeForFileExtension(_uploadFile));
                    request.SupportsTeamDrives = true;
                    //request.ProgressChanged += Request_ProgressChanged;
                    //request.ResponseReceived += Request_ResponseReceived;
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception ex)
                {
                    throw ex; 
                    return null;
                }
            }
            else
            {               
                return null;
            }
        }
        //private string GetMimeType(string fileName)
        //{
        //    string mimeType = "application/unknown";
        //    string ext = System.IO.Path.GetExtension(fileName).ToLower();
        //    Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
        //    if (regKey != null && regKey.GetValue("application/octet-stream") != null)
        //        mimeType = regKey.GetValue("application/octet-stream").ToString();
        //    return mimeType;
        //}
        private string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }

        [HttpPost("SendMail")]
        [AllowAnonymous]
        [Authorize]
        //public async Task<ServiceResponse<Resource>> SendMail(string to, string subject, string body)
        public async Task<ServiceResponse<Resource>> SendMail()
        {
            var sr = new ServiceResponse<Resource>();
            try
            {
                string credentialsPath = _environment.ContentRootPath;
                string path = Path.Combine(_environment.ContentRootPath, "wwwroot\\Jsonfiles\\client_secret_GmailAPI.json");

                GmailService service = InitializeGmailService(path);

                // Send an email
                bool result = SendEmail(service, "shardap.brainerhub@gmail.com", "Sent Mail Demo", "Hello World");
                if (result == true)
                {
                    sr.Message = "Email sent successfully.";
                }
                else
                {
                    sr.Message = "Email not sent.";
                }
                return sr;
            }
            catch (Exception ex)
            {
                sr.Message = $"An error occurred: {ex.Message}";
                throw ex;
            }
        }
        private GmailService InitializeGmailService(string credentialsPath)
        {
            UserCredential credential;

            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";  // Path to store the user's credentials
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { GmailService.Scope.GmailSend },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Gmail API service
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gmail API Example"
            });

            return service;
        }
        private bool SendEmail(GmailService service, string to, string subject, string body)
        {
            bool result = false;
            var msg = new Message
            {
                Raw = Base64UrlEncode(CreateEmail(to, "shardap.brainerhub@gmail.com", subject, body))
            };

            try
            {
                service.Users.Messages.Send(msg, "me").Execute();
                Console.WriteLine("Email sent successfully.");
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return result;
        }

        static Google.Apis.Gmail.v1.Data.Message CreateEmail(string to, string from, string subject, string body)
        {
            var msg = new Google.Apis.Gmail.v1.Data.Message();
            var utf8Bytes = System.Text.Encoding.UTF8.GetBytes(body);
            var base64 = Convert.ToBase64String(utf8Bytes);
            msg.Raw = base64;

            msg.Payload = new MessagePart
            {
                Headers = new[] {
                    new MessagePartHeader { Name = "To", Value = to },
                    new MessagePartHeader { Name = "From", Value = from },
                    new MessagePartHeader { Name = "Subject", Value = subject }
                }
            };

            return msg;
        }
        private string Base64UrlEncode(Google.Apis.Gmail.v1.Data.Message message)
        {
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message.ToString());
            return Convert.ToBase64String(messageBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
    }
}
