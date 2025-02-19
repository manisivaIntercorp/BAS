using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer.Model
{
        public class AuditLog
    {
            public long Id { get; set; }
            public long? UserId { get; set; }
            public string? UserGuid { get; set; }
            public string? Mode { get; set; }
            public string? Token { get; set; }
            public string? Action { get; set; }
            //public string ActionType { get; set; } = string.Empty;
            //public string? TableName { get; set; }
            //public string? OldValues { get; set; }
            //public string? NewValues { get; set; }
            //public string? AffectedColumns { get; set; }
            //public DateTime Timestamp { get; set; } = DateTime.UtcNow;
            public string? IPAddress { get; set; }
            public string? DeviceInfo { get; set; }
            public string? CreatedBy { get; set; }
            public DateTime? CreatedDateTime { get; set; }
            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedDateTime { get; set; }
            public DateTime? UTCCreatedDateTime { get; set; }
            public DateTime? UTCModifiedDateTime { get; set; }
    }    
}
