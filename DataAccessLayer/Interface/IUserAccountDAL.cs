using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IUserAccountDAL
    {
        Task<List<UserAccountModel>> GetAllUserAccount();
        Task<bool> InsertUpdateUserAccount(UserAccountModel UM);
        Task<bool> DeleteUserAccount(int Id);
        Task<UserAccountModel> GetUserAccountById(int Id);
        Task<bool> UpdateUserAccountAsync(int id, UserAccountModel userAccount);
    }
}
