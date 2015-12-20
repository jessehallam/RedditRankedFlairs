using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace Hallam.RedditRankedFlairs.WebAPI
{
    /// <summary>
    ///     A WebAPI message handler which transforms response messages in to a common format.
    /// </summary>
    public class ApiMessageHandler : DelegatingHandler
    {
        private static readonly MediaTypeFormatter JsonFormatter = new JsonMediaTypeFormatter
        {
            SerializerSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}
        };

        /// <summary>
        ///     Data transfer object for WebAPI response messages.
        /// </summary>
        private class ResponseDto
        {
            [JsonProperty("result")]
            public object Content { get; set; }

            [JsonProperty("error")]
            public string Error { get; set; }

            [JsonProperty("exception")]
            public object Exception { get; set; }

            [JsonProperty("status")]
            public int StatusCode { get; set; }

            [JsonProperty("validationErrors")]
            public string[] ValidationErrors { get; set; }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return GetResponseMessage(request, await base.SendAsync(request, cancellationToken));
        }

        private HttpResponseMessage GetResponseMessage(HttpRequestMessage request, HttpResponseMessage response)
        {
            var result = new ResponseDto {StatusCode = (int) response.StatusCode};
            object content;

            if (response.TryGetContentValue(out content))
            {
                var error = content as HttpError;

                if (error != null)
                {
                    result.Error = error.Message;

                    if (!string.IsNullOrEmpty(error.ExceptionMessage))
                    {
#if DEBUG
                        result.Exception = ResolveExceptionDto(error);
#endif
                        if (result.Error == "An error has occurred.")
                            result.Error = "Internal server error.";
                    }
                    else if (error.ModelState != null)
                    {
                        result.ValidationErrors =
                            error.ModelState.SelectMany(kvp => (IEnumerable<string>) kvp.Value).ToArray();
                    }
                }
                else
                {
                    result.Content = content;
                }
            }

            response.Content = new ObjectContent(typeof (ResponseDto), result, JsonFormatter);
            return response;
        }

        private object ResolveExceptionDto(HttpError error)
        {
            if (error == null) return null;
            dynamic r = new ExpandoObject();
            r.exceptionType = error.ExceptionType;
            r.exceptionMessage = error.ExceptionMessage;
            r.stackTrace = error.StackTrace;
            r.innerException = ResolveExceptionDto(error.InnerException);
            return r;
        }
    }
}