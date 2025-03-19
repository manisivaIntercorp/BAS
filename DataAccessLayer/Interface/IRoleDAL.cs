using DataAccessLayer.Model;

namespace DataAccessLayer.Interface
{
    public interface IRoleDAL
    {
        Task<List<GetRoleModel>> GetAllRole(long UpdatedBy);
        
        Task<(List<RoleModel?> roleModels, long? RetVal,string? Msg)> InsertUpdateRole(RoleModel? LM);
        Task<(List<GetRoleModel?> roleModels, long? RetVal, string? Msg)> UpdateRole(GetRoleModel? LM);
        Task<(bool? DeleteRole, List<DeleteRoleInformation?> deleteRoleInformation)> DeleteRole(RolesDelete? rolesDelete,long UserId );
        Task<(RoleModel? rolemodel,List<Modules?> ModuleDatatable)> getModulesBasedOnRole(string? RoleGUID, long? UserGUID );
        Task<List<Modules?>> getModulesBasedOnInsertRole(long? UserGUID);
        Task<(List<GetRoleModel?> roleModels, long? RetVal, string? Msg)> EditUpdateRoleAsync(GetRoleModel roleModel);
    }
}
