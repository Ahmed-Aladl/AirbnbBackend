using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Result
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? StatusCode { get; set; }

        protected Result(bool isSuccess, string message, int statusCode)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public static Result Success(int statusCode=200, string message = "Success")
            => new(true, message, statusCode);

        public static Result Fail(string message, int statusCode)
            => new(false, message,statusCode);
    }
}
