//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Responses
//{

//    public class Result
//    {
//        public bool IsSuccess { get; set; }
//        public string Message { get; set; } = string.Empty;
//        public int? StatusCode { get; set; }

//        protected Result(bool isSuccess,string message) 
//        {
//            IsSuccess = isSuccess;
//            Message = message;
//        }
//        public static Result Success( string message = "Success")
//            => new(true,message);

//        public static Result Fail(string message)
//            => new(false,message);
//    }
//    public class Result<T>:Result
//    {
//        public T Data { get; set; }

//        protected Result(T? data,bool isSuccess,string message)
//            : base(isSuccess,message) 
//        {
//            Data = data;

//        }
//        public static Result<T> Success(T data, string message = "Success")
//            => new(data,true,message);

//        public static new Result<T> Fail(string message)
//            => new(default, false,message);
//    }






//    public class ApiResponse<T>
//    {
//        public bool IsSuccess { get; set; }
//        public string Message { get; set; } = string.Empty;
//        public int? StatusCode { get; set; }

//        public T? Data { get; set; }

//        public static ApiResponse<T> Success(T data, string message = "Success")
//            => new() { IsSuccess = true, Message = message, Data = data };

//        public static ApiResponse<T> Fail(string message)
//            => new() { IsSuccess = false, Message = message, Data = default };
//    }
//}
