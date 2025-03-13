using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class OrganisationModel
    {
        public long ID { get; set; }
        public string? OrgCode { get; set; }
        public string? OrgName { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? PinCode { get; set; }
        public string? PriContactNo { get; set; }
        public string? SecContactNo { get; set; }
        public string? PriEmailAddress { get; set; }
        public string? SecEmailAddress { get; set; }
        public string? CcEmailAddress { get; set; }
        public string? Logo { get; set; }
        public string? DBName { get; set; }
        public long? DataLocationID { get; set; }
        public string? SepPolling { get; set; }
        public string? Active { get; set; }
        public long? UserID { get; set; }

        public DateTime? LastActiveDate { get; set; }
        public string? Guid { get; set; }

        //public DataTable? FunctionConfiguration { get; set; } // Matches utt_FunctionConfiguration
        //public DataTable? Modules { get; set; } // Matches utt_FunctionConfiguration
        //public DataTable? DeleteRecords { get; set; }

        public List<DeleteRecord> DeleteRecords { get; set; } = new();
        public List<FunctionConfiguration> FunctionConfigurations { get; set; } = new();
    }

    public class OrganisationDeleteRecord()
    {
        public int? SNo { get; set; }
        public string? OrgCode { get; set; }
        public string? OrgName { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
    }



    public class DeleteRecord
    {
        public string? Guid { get; set; }
        public string? FldInfo { get; set; }
    }

    public class FunctionConfiguration
    {
        public long ID { get; set; }
        public string? Required { get; set; }
    }
    public class DataLocationDropdown
    {
        public long Value { get; set; }
        public string? Text { get; set; }
    }
}
