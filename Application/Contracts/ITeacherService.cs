using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts;

public interface ITeacherService
{
    Task AssignTeacherRoleToUser(string Id);

    Task<User?> BlockUserByIdAsync(string Id);
}
