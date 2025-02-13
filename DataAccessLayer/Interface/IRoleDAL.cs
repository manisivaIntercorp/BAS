using DataAccessLayer.Model;

namespace DataAccessLayer.Interface
{
    public interface IRoleDAL
    {
        Task<List<RoleModel>> GetAllRole(int UserId);
        Task<(List<RoleModel?> roleModels, int? RetVal,string? Msg)> InsertUpdateRole(RoleModel? LM);
        Task<(bool DeleteRole, List<DeleteRoleInformation> deleteRoleInformation)> DeleteRole(int Id,int UserId );
        Task<(RoleModel? rolemodel,List<Modules?> ModuleDatatable)> getModulesBasedonRole(long? Roleid, string? IsPayrollAccessible,long? UserID );
        Task<(List<RoleModel?> roleModels, int? RetVal, string? Msg)> UpdateRoleAsync(int id, RoleModel roleModel);
    }
}
