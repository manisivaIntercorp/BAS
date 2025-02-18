using DataAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ILoginDAL
    {
        
        Task<List<ResultModel>> ClientUserLogin(LoginModel loginModel);
        Task<List<ResultModel>> UserLogin(LoginModel loginModel);
        Task<List<ResultModel>> GetUserID(LoginModel loginModel);
        Task<List<OrganisationDBDetails>> GetOrganisationWithDBDetails(LoginModel loginModel);

        Task<List<GetDropDownDataModel>> GetDDlLanguage(string Mode, string RefID1, string RefID2, string RefID3);
        Task<List<GetDropDownDataModel>> GetDDlModule(string Mode, string RefID1, string RefID2, string RefID3);

        Task<bool> InsertLoginDetails(LoginDetails loginDetails);
        Task<bool> UpdateLoginDetails(LoginDetails loginDetails);

    }
}
