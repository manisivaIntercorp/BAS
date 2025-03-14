using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class MenuModel
    {    
            public long ModuleID { get; set; }
            public string? ModuleCode { get; set; }
            //public string? Level1 { get; set; }
            //public string? Level2 { get; set; }
            //public string? Level3 { get; set; }
            public string? ModuleName { get; set; }
            public long? ParentID { get; set; }
            public long? MenuOrder { get; set; }
            public long? Selected { get; set; }
            public string? DisplayName { get; set; }
            public string? PageLink { get; set; }
            public string? MenuGroup { get; set; }
            public string? ModuleType { get; set; }
            //public long? CreatedBy { get; set; }
            //public DateTime? CreatedDateTime { get; set; }
            //public long? ModifiedBy { get; set; }
            //public DateTime? ModifiedDateTime { get; set; }
            public long? FunctionID { get; set; }
            public string? Active { get; set; }
            public string? IconStyle { get; set; }


    }

    public class MenusModel
    {
        public int MenuId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ParentId { get; set; }
        public string? Icon { get; set; }
        public string? Controller { get; set; }
        public string? ControllerAction { get; set; }
        public int MenuOrder { get; set; }
    }
}
