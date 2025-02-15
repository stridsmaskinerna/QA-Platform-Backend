namespace Infrastructure.Seeds;

public class SeedException(
    string message = "An unexpected error occurred during seeding."
) : Exception(message)
{ }
