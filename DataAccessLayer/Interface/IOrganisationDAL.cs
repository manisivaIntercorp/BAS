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

        Task<OrganisationModel> GetOrganisationById(int Id);


        Task<string> InsertOrganisation(OrganisationModel OrganisationModel);
        Task<string> UpdateOrganisation(int id, OrganisationModel OrganisationModel);

        Task<List<OrganisationDeleteRecord>> DeleteOrganisation(List<DeleteRecord> dltOrg);
    }
}
