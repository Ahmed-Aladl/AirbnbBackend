using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Airbnb.Middleware
{
    public static class ModelStateExtensions
    {
        public static List<string> ExtractErrorList(this ModelStateDictionary modelState)
        {
            return modelState
                .Values.SelectMany(v => v.Errors)
                .Select(e =>
                    string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid input" : e.ErrorMessage
                )
                .ToList();
        }
    }
}
