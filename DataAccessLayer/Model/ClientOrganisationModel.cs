using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class ClientOrganisationModel
    {
    }

    public class LevelInfoModel
    {
        public long? LevelID { get; set; }
        public string? LevelCode { get; set; }
        public string? LevelDesc { get; set; }
        public string? ParentLevel { get; set; }
        public string? LastLevelApplicable { get; set; }
        public string? ChildAvailable { get; set; }
        public string? IsProject { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedDateTime { get; set; }
        public string? Guid { get; set; }
        public string? ParentLevelGuid { get; set; }
    }
    public class OrganisationLevelModel
    {
        public string? Mode { get; set; }
        public long? LevelID { get; set; }
        public string? LevelGuid { get; set; }
        public string? LevelCode { get; set; }
        public string? LevelDesc { get; set; }
        public string? ParentLevelGuid { get; set; }
        public long? ParentLevelID { get; set; }
        public string? ParentLevelName { get; set; }
        public string? IsProject { get; set; }
        public long? ModifiedBy { get; set; }     
        public string? UserGuid { get; set; }
        public List<LevelInfoDetails>? LevelInfo { get; set; } // For TVP
    }

    public class LevelInfoDetails
    {
        public string? LevelInfo { get; set; }
        public long? LevelID { get; set; }
        public string? Guid { get; set; }
    }

    public class DeleteResultModel
    {
        public long? SNo { get; set; }
        public string? LevelInfo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
    }
}
