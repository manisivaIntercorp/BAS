

using System.Data;
using System.Text.Json.Serialization;

namespace DataAccessLayer.Model
{
    public class RolesDelete
    {
        public List<RolesDeleteInList> DeleteRoleNames { get; set; } = new List<RolesDeleteInList>();
        public DataTable DeleteRoleTable = new DataTable();
        public DataTable ConvertToDataTable(List<RolesDeleteInList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(RolesDeleteInList).GetProperties();
            foreach (var property in properties)
            {
                DeleteRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = DeleteRoleTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                DeleteRoleTable.Rows.Add(row);
            }

            return DeleteRoleTable;
        }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
        
    }
    public class RolesDeleteInList
    {
        public string? RoleGUID { get; set; }
    }
}
