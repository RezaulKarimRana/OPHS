using System;

namespace Infrastructure.HttpClients.Models
{
    public class InternalServiceErrorException : Exception
    {
        public string RequestUri { get; set; }

        public string ResponseMessage { get; set; }

        public InternalServiceErrorException(string requestUri, string responseMessage) : base($"An unexpected error was returned from '{requestUri}'")
        {
            RequestUri = requestUri;
            ResponseMessage = responseMessage;
        }
    }

}
