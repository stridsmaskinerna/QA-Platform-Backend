namespace Domain.DTO.Response;

public class TokenDTO
{
    public string accessToken { get; set; } = string.Empty;
    public string? refreshToken { get; set; } = "";
}
