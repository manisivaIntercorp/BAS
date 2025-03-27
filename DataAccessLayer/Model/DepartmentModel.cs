using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class DepartmentModel
    {
        public List<DeptDetails>? lstDeptDetails { get; set; }

        public List<Division>? lstDivision { get; set; }
        public List<Category>? lstCategory { get; set; }
        public List<DivisionSelection>? lstDivisionSelection { get; set; }
        public List<CategorySelection>? lstCategorySelection { get; set; }
    }

    public class DeptDetails
    {
        public string? Guid { get; set; }
        public string? DeptCode { get; set; }
        public string? DeptDesc { get; set; }
        public string? DivCodeDesc { get; set; }
        public string? ReferenceID { get; set; }
        public string? Function { get; set; }
        public string? Active { get; set; }
        public DateTime? LastActiveDate { get; set; }
        public string? LevelDetailsGUID { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedDateTime { get; set; }
        public string? Color { get; set; }
    }


    public class Division
    {
        public long? DivisionID { get; set; }
        public string? DivisionGUID { get; set; }
        public string? DivisionDesc { get; set; }
    }

    public class Category
    {
        public long? CategoryID { get; set; }
        public string? CategoryGUID { get; set; }
        public string? CategoryDesc { get; set; }
        public string? Function { get; set; }
    }

    public class DivisionSelection
    {
        public long? DivisionID { get; set; }
        public string? DivisionGUID { get; set; }
        public string? DivisionDesc { get; set; }
        public string? Selected { get; set; } // If "Selected" is a flag, consider using a bool type instead
    }

    public class CategorySelection
    {
        public long? CategoryID { get; set; }
        public string? CategoryGUID { get; set; }
        public string? DivisionGUID { get; set; }
        public string? CategoryDesc { get; set; }
        public string? Selected { get; set; }
    }

    public class DepartmentDetail
    {
        public string? DeptGUID { get; set; }
        public string? DivisionGUID { get; set; }
        public string? CategoryGUID { get; set; }
        public string? Department { get; set; }
    }

    public class DepartmentInput
    {
        public string? Mode { get; set; }
        public string? UpdatedGuidBy { get; set; }
        public string? LevelDetailGUID { get; set; }
        public string? DeptCode { get; set; }
        public string? DeptDesc { get; set; }
        public string? LevelGUID { get; set; }
        public string? DeptGUID { get; set; }
        public string? ColourCode { get; set; }

        public long? ReferenceID { get; set; }
        public string? Reference_ID { get; set; }
        public string? Function { get; set; }
        public long? TimeZoneID { get; set; }
        public List<DepartmentDetail>? lstDepart { get; set; }
    }
    public class DeptDeleteResult
    {
        public long? SNo { get; set; }
        public string? DepartmentCode { get; set; }
        public string? RESULT { get; set; } 
        public string? REMARKS { get; set; } 
    }
}
