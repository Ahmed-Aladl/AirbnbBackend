using Application.Result;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
                return new ObjectResult(result);
            //if (result.IsSuccess)
            //{
            //}

            //return new ObjectResult(new
            //{
            //    Success = false,
            //});
        }
    }

}
