using Microsoft.AspNetCore.Mvc;

namespace Playground;

public class ProblemDetailsException : Exception
{
    public ProblemDetails ProblemDetails { get; }

    public ProblemDetailsException(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails;
    }
}
