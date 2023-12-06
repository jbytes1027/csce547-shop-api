namespace FU.API.Exceptions
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public abstract class ExceptionWithResponse : Exception
    {
        public abstract string Description { get; }

        public abstract string Title { get; }

        public abstract HttpStatusCode StatusCode { get; }

        public ProblemDetails GetProblemDetails()
        {
            return new()
            {
                Title = Title,
                Detail = Description,
                Status = (int)StatusCode,
            };
        }
    }

    public class NotFoundException : ExceptionWithResponse
    {
        public override string Description { get; } = "The requested resource was not found.";

        public override string Title { get; } = "Not found";

        public override HttpStatusCode StatusCode { get; } = HttpStatusCode.NotFound;

        public NotFoundException()
        {
        }

        public NotFoundException(string description)
        {
            Description = description;
        }
    }

    public class UnprocessableContentException : ExceptionWithResponse
    {
        public override string Description { get; } = "Content unprocessable";

        public override string Title { get; } = "Content unprocessable";

        public override HttpStatusCode StatusCode { get; } = HttpStatusCode.UnprocessableEntity;

        public UnprocessableContentException()
        {
        }

        public UnprocessableContentException(string description)
        {
            Description = description;
        }
    }

    public class ConflictException : ExceptionWithResponse
    {
        public override string Description { get; } = "A conflict occured with the requested resource.";

        public override string Title { get; } = "Conflict";

        public override HttpStatusCode StatusCode { get; } = HttpStatusCode.Conflict;

        public ConflictException()
        {
        }

        public ConflictException(string description)
        {
            Description = description;
        }
    }
}