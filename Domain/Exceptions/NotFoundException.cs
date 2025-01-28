namespace Domain.Exceptions;

public class NotFoundException(
    string message = "",
    string title = "Not Found"
) : ApiException(message, title, 404)
{ }
