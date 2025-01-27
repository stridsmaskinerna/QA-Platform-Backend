using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    public class UserDTO
    {
        public string Id = String.Empty;
        public string UserName = String.Empty;
    }


    public class UserWithEmailDTO : UserDTO{
        public string Email;

    }

    public class UserDetailsDTO : UserWithEmailDTO 
    {
        public bool IsBlocked;

    }
}
