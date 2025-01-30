using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO.Response
{
    public class TokenDTO
    {
        public string accessToken { get; set; } = string.Empty;
        public string? refreshToken { get; set; } = "";
    }
}
