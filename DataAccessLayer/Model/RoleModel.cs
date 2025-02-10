using System.Data;

namespace DataAccessLayer.Model
{
    public class RoleModel
    {
        public DataTable ModuleTable = new DataTable();
        public DataTable ConvertToDataTable(List<ModulesDatatable> models)
        {
            
            // Define columns dynamically based on the model's properties
            var properties = typeof(ModulesDatatable).GetProperties();
            ModuleTable.Columns.Add("RoleID",typeof(Int64));
            foreach (var property in properties)
            {
                ModuleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = ModuleTable.NewRow();
                foreach (var property in properties)
                {
                    row["RoleID"] = 0;
                    row[property.Name] = property.GetValue(model)??DBNull.Value;
                }
                ModuleTable.Rows.Add(row);
            }
            return ModuleTable;
        }
        public long RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? IsAdmin { get; set; }
        public string? IsEntityAdmin { get; set; }
        public string? Active { get; set; }
        public long CreatedBy { get; set; }
        public string? IsPayrollAccessible { get; set; }
        public string? DisplayPDPAData { get; set; }
        public int LevelID { get; set; }
        public int LevelDetailsID { get; set; }
        public string? AccessToAllClient { get; set; }
    }

    public class ModulesDatatable
    {
        public string? RARMode { get; set; }
        public Int64 ModuleID { get; set; }
        public string? Selected { get; set; }
    }

    public class Modules
    {
        public string? SelectedMode { get; set; }
        
        public string? ItemID { get; set; }
        public string ? ItemDesc { get; set; }
        public string ? ParentID { get; set; }
        public string? Type { get; set; }
        public string? Selected { get; set;}
        public string? Color { get; set; }
        public long MenuOrder { get; set; }
        public long FunctionID { get; set; }
        public string? code { get; set; }
        
    }
    public class RoleUpdateRequest
    {
        public RoleModel? RoleModel { get; set; }
        public List<ModulesDatatable>? ModuleDatatable { get; set; }
    }
    public class GetRoleUpdateRequest
    {
        public RoleModel? RoleModel { get; set; }
        public List<Modules?> ModuleDatatable { get; set; }
    }

}
