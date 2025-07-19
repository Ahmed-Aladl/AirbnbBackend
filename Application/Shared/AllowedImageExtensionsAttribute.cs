using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Shared
{
    public class AllowedImageExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedImageExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult(
                        ErrorMessage
                            ?? $"This file type is not allowed. Allowed: {string.Join(", ", _extensions)}"
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
}
