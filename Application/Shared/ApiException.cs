using System.Net;

namespace Application.Shared
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public bool IsClientError { get; }
        public ApiException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, bool isClientError = false)
    : base(message)
        {
            StatusCode = statusCode;
            IsClientError = isClientError;
        }
    }

    public class NotFoundException : ApiException
    {
        public NotFoundException(string message) : base(message, HttpStatusCode.NotFound, true)
        { }
    }

    public class ConflictException : ApiException
    {
        public ConflictException(string message)
            : base(message, HttpStatusCode.Conflict, true) { }
    }


    public class ValidationException : ApiException
    {
        public List<string> Errors { get; }

        public ValidationException(List<string> errors)
            : base("Validation failed", HttpStatusCode.BadRequest, true)
        {
            Errors = errors;
        }

        public ValidationException(string message) : this([message]) { }
    }

    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message)
            : base(message, HttpStatusCode.Unauthorized, true) { }
    }

    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message)
            : base(message, HttpStatusCode.Forbidden, true) { }
    }
}
