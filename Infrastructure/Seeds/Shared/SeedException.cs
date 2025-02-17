namespace Infrastructure.Seeds.Shared;

public class SeedException(
    string message = "An unexpected error occurred during seeding."
) : Exception(message)
{ }
