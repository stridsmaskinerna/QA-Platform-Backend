using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class User : IdentityUser
{
    public bool IsBlocked { get; set; }

    //Navigation
}
