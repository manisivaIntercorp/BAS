using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ILoginDAL
    {
        Task<List<ResultModel>> UserLogin(LoginModel loginModel);
        Task<List<ResultModel>> GetUserID(LoginModel loginModel);
        Task<List<OrganisationDBDetails>> GetOrganisationWithDBDetails(LoginModel loginModel);
    }
}
