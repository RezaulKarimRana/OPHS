using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMS.Models.ViewModel
{
    public enum ResponseStatus
    {
        Ok = 0,
        NotFound = 1,
        Duplicate = 2,
        BadRequest = 3,
        Conflict = 4,
        PassErrorToClient = 100
    }
    public class ResponseViewModel
    {
        [JsonIgnore]
        public ResponseStatus Status { get; set; }

        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public bool HasNextPage { get; set; }

        public string GetErrorMessage()
        {
            return ErrorMessages.Any() ? string.Join(Environment.NewLine, ErrorMessages) : "";
        }
        public ResponseViewModel() { }
        public ResponseViewModel(string errorMessage)
        {
            Success = false;
            ErrorMessages.Add(errorMessage);
        }
        public ResponseViewModel(bool success, params string[] errorMessages)
        {
            Success = success;
            if (errorMessages != null)
                ErrorMessages.AddRange(errorMessages);
        }
    }
    public class ResponseViewModel<T> : ResponseViewModel
    {
        public ResponseViewModel()
        {

        }
        public ResponseViewModel(string errorMessage, ResponseStatus? rs = null) : base(errorMessage)
        {
            Success = false;
            if (rs != null)
                this.Status = rs.Value;
        }

        public ResponseViewModel(T data)
        {
            Data = data;
        }
        public ResponseViewModel(T data, bool success)
        {
            Data = data;
            Success = success;
        }
        public ResponseViewModel(bool success)
        {
            Success = success;
        }

        public T Data { get; set; }
    }
}
