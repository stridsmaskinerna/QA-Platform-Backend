using System.ComponentModel.DataAnnotations;

namespace Domain.DTO;

public class UserDTO
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}


public class UserWithEmailDTO : UserDTO
{
    public string Email { get; set; } = string.Empty;

}

public class UserDetailsDTO : UserWithEmailDTO
{
    public bool IsBlocked { get; set; }

}
