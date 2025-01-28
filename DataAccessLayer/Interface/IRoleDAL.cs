using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IRoleDAL
    {
        Task<List<RoleModel>> GetAllRole();
        Task<(List<RoleModel> roleModels, int RetVal,string Msg)> InsertUpdateRole(RoleModel LM);
        Task<bool> DeleteRole(int Id);
        Task<(RoleModel rolemodel,List<Modules> ModuleDatatable)> getModulesBasedonRole(long Roleid, string IsPayrollAccessible,long UserID );
        Task<(List<RoleModel> roleModels, int RetVal, string Msg)> UpdateRoleAsync(int id, RoleModel roleModel);
    }
}
