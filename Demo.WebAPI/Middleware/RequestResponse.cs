using Demo.Contract.Services.Shared;
using System.Text;

namespace Demo.WebAPI.Middleware
{
    public class RequestResponse
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService _logger;

        public RequestResponse(RequestDelegate next, ILoggerService logger)
        {
            _next = next;
            _logger = logger;
        }

        //https://alexbierhaus.medium.com/api-request-and-response-logging-middleware-using-net-5-c-a0af639920da#:~:text=Middleware%20is%20used%20when%20you,%E2%80%9Cplugin%E2%80%9D%20your%20custom%20middleware.
        public async Task InvokeAsync(HttpContext context)
        {
            // Copy  pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Get incoming request
            var body = context.Request.Body;

            //Set the reader for the request back at the beginning of its stream.
            context.Request.EnableBuffering();

            //Read request stream
            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];

            //Copy into  buffer.
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);

            //Convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //Assign the read body back to the request body
            context.Request.Body = body;

            var ep =  $"{context.Request.Scheme} {context.Request.Host}{context.Request.Path} {context.Request.QueryString} {bodyAsText}";
            //var request = await GetRequestAsTextAsync(context.Request);
            //Log it
            _logger.LogInfo(ep);

            //Create a new memory stream and use it for the temp reponse body
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            //Continue down the Middleware pipeline
            await _next(context);

            //Format the response from the server
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            //Create stream reader to write entire stream
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = text;
            _logger.LogError(response);

            //Copy the contents of the new memory stream, which contains the response to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalBodyStream);
        }        
    }
}
