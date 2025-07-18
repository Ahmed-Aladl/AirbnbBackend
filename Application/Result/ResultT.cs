using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Result
{
    public class Result<T> : Result
    {
        public T Data { get; set; }

        protected Result(T? data, bool isSuccess, string message, int statusCode)
            : base(isSuccess, message, statusCode)
        {
            Data = data;

        }
        public static Result<T> Success(T data, int statusCode=200, string message = "Success")
            => new(data, true, message, statusCode);

        public static new Result<T> Fail(string message, int statusCode)
            => new(default, false, message, statusCode);
    }
}
