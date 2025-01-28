namespace Domain.Exceptions;

public abstract class ApiException(
    string message = "An unexpected error occurred.",
    string title = "Internal Server Error",
    int statusCode = 500
) : Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
    public string Title { get; set; } = title;
    public string Detail { get; set; } = message;
}
