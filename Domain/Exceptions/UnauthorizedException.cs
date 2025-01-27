namespace Domain.Exceptions;

public class UnauthorizedException(
    string message = "",
    string title = "Unauthorized"
) : ApiException(message, title, 401)
{ }
