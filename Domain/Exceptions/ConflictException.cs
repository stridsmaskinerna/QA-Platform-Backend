namespace Domain.Exceptions;

public class ConflictException(
    string message = "",
    string title = "Conflict"
) : ApiException(message, title, 409)
{ }
