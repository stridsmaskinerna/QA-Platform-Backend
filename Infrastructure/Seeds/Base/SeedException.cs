namespace Infrastructure.Seeds.Base;

public class SeedException(
    string message = "An unexpected error occurred during seeding."
) : Exception(message)
{ }
