namespace Domain.Exceptions;

public class ForbiddenException(
    string message = "",
    string title = "Forbidden"
) : ApiException(message, title, 403)
{ }
