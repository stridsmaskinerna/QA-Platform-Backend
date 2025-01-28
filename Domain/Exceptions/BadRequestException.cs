namespace Domain.Exceptions;

public class BadRequestException(
    string message = "",
    string title = "Bad Request"
) : ApiException(message, title, 400)
{ }
