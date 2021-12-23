using System;
using System.Collections.Generic;
using System.Text;

namespace RaspberryPi.Web.DTOs
{
    public abstract class ApiResponsetBase
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

    public class ApiResponse : ApiResponsetBase
    {
        public ApiResponse()
        {

        }
        
        public ApiResponse(bool status, string message, int statusCode)
        {
            Status = status;
            Message = message;
            StatusCode = statusCode;
        }
        public ApiResponse(bool status)
        {
            Status = status;
            Message = "";
            StatusCode = 0;
        }
        public ApiResponse(bool status, string message)
        {
            Status = status;
            Message = message;
            StatusCode = 0;
        }

        public override bool Equals(object obj)
        {
            return obj is ApiResponse response &&
                   Status == response.Status &&
                   Message == response.Message &&
                   StatusCode == response.StatusCode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Status, Message, StatusCode);
        }
    }

    public class ApiResponse<T> : ApiResponsetBase
    {
        public ApiResponse()
        {

        }
       
        public ApiResponse(bool status, string message, int statusCode, T data)
        {
            Status = status;
            Message = message;
            StatusCode = statusCode;
            Data = data;
        }
        public ApiResponse(bool status, T data)
        {
            Status = status;
            Message = "";
            StatusCode = 0;
            Data = data;
        }
        public ApiResponse(bool status, string message, T data)
        {
            Status = status;
            Message = message;
            StatusCode = 0;
            Data = data;
        }

        public T Data { get; set; }
    }
}
