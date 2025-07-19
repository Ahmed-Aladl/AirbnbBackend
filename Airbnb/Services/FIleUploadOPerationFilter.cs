using Application.DTOs.PropertyImageDTOs;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Airbnb.Services
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        //public void Apply(OpenApiOperation operation, OperationFilterContext context)
        //{
        //    if (operation.RequestBody != null)
        //    {
        //        operation.RequestBody.Content["multipart/form-data"] =
        //            operation.RequestBody.Content["application/json"];
        //        operation.RequestBody.Content.Remove("application/json");
        //    }
        //}
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formParams = context.MethodInfo
                .GetParameters()
                .Where(p => p.ParameterType == typeof(PropertyImagesUploadContainerDTO))
                .ToList();

            //if (operation.RequestBody != null &&
            //            operation.RequestBody.Content.ContainsKey("application/json"))
            //    {
            //        operation.RequestBody.Content["multipart/form-data"] =
            //            operation.RequestBody.Content["application/json"];

            //        operation.RequestBody.Content.Remove("application/json");
            //    }

        }
    }

}
