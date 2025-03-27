using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IClientOrganisationDAL
    {
        Task<List<OrganisationModel>> GetClientOrganisation();
        Task<List<LevelInfoModel>> GetOrganisationLevelInfo(OrganisationLevelModel organisationLevelModel);
        Task<string> OrganisationLevelEdit(OrganisationLevelModel organisationLevelModel);
        Task<List<LevelInfoModel>> SelectOrganisationLevelInfo(OrganisationLevelModel organisationLevelModel);
        Task<List<DeleteResultModel>> DeleteOrganisationLevel(List<LevelInfoDetails> levelInfoDetails, string struserGuid, string strMode);
    }
}
