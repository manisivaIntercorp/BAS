using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IDepartmentDAL
    {
        Task<DepartmentModel> GetDepartment(DepartmentInput departmentInput);  // ✅ No 'public' needed
        Task<DepartmentModel> ViewDepartment(DepartmentInput departmentInput);  // ✅ No 'public' needed

        Task<string> InsertDepartmentDetails(DepartmentInput organisationLevelModel);
        Task<string> UpdateDepartmentDetails(DepartmentInput organisationLevelModel);
        Task<List<DeptDeleteResult>> DeleteDepartmentDetails(DepartmentInput departmentInput);
        
    }

}
