using DataAccessLayer.Model;


namespace DataAccessLayer.Interface
{
    public interface IUserGroupDAL
    {
        Task<List<GetUserGroupModel?>> GetAllUserPolicy(long UpdatedBy);
        Task<(bool InsertUserGroup, int RetVal, string Msg)> InsertUpdateUserPolicy(UserGroupModel? UM);

        Task<(bool deleteuserGroup, List<DeleteUserGroupResult> deleteResults)> DeleteUserPolicy(long UpdatedBy,DeleteUserGroup deleteUserGroup);
        Task<GetUserGroupModel?> GetUserPolicyByGUId(string? GUId);
        Task<(bool UpdateUserGroup, int RetVal, string Msg)> UpdateUserPolicyAsync(UpdateUserGroupModel? userAccount);
    }

}
