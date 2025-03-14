using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IOrganisationDAL
    {
        Task<List<OrganisationModel>> GetAllOrganisation();

        Task<OrganisationModel> GetOrganisationById(string Guid);


        Task<string> InsertOrganisation(OrganisationModel OrganisationModel);
        Task<string> UpdateOrganisation(OrganisationModel OrganisationModel);

        Task<List<OrganisationDeleteRecord>> DeleteOrganisation(List<DeleteRecord> dltOrg);
        Task<List<DataLocationDropdown>> DataLocationInDropdown();
        Task<List<OrganisationModules>> GetAllModules();
    }
}
