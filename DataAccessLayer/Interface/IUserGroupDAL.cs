using DataAccessLayer.Model;


namespace DataAccessLayer.Interface
{
    public interface IUserGroupDAL
    {
        Task<List<UserGroupModel?>> GetAllUserPolicy();
        Task<bool> InsertUpdateUserPolicy(UserGroupModel? UM);
        Task<bool> DeleteUserPolicy(int Id);
        Task<UserGroupModel?> GetUserPolicyById(int? Id);
        Task<bool> UpdateUserPolicyAsync(int? id, UserGroupModel? userAccount);
    }

}
