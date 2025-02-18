using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Contracts;

namespace Application.Services
{
    public class TeacherService : BaseService, ITeacherService
    {
        public readonly IRepositoryManager _rm;

        public TeacherService(IRepositoryManager rm)
        {
            _rm = rm;
        }

        public async Task AssignTeacherRoleToUser(string Id)
        {
            await _rm.UserRepository.ChangeUserRoleToTeacher(Id);
        }

        public async Task BlockUserByIdAsync(string Id)
        {
            await _rm.UserRepository.BlocKUserById(Id);
        }
    }
}
