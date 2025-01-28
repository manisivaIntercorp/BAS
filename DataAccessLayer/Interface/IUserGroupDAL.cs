using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IUserGroupDAL
    {
        Task<List<UserGroupModel>> GetAllUserPolicy();
        Task<bool> InsertUpdateUserPolicy(UserGroupModel UM);
        Task<bool> DeleteUserPolicy(int Id);
        Task<UserGroupModel> GetUserPolicyById(int Id);
        Task<bool> UpdateUserPolicyAsync(int id, UserGroupModel userAccount);
    }

}
