using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "")
            => new() { IsSuccess = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message)
            => new() { IsSuccess = false, Message = message, Data = default };
    }
}
