using System.Data;
using System.Text.Json.Serialization;

namespace DataAccessLayer.Model
{
    public class NationalityModel
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? Nationality { get; set; }
        public string? NationalityCode { get; set; }
        public string? Active { get; set; }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        
    }
    public class GetNationalityModel
    {
        
        public int Id { get; set; }
        public string? Nationality { get; set; }
        public string? NationalityCode { get; set; }
        public string? Active { get; set; }
        public string? CreatedName { get; set; }
        public string? ModifiedName { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        
        public DateTime? ModifiedDateTime { get; set; }

        public DateTime? LastActiveDate { get; set; }
        public string? NationalityGUID { get; set; }
    }
    public class UpdateNationality
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? Nationality { get; set; }
        public string? NationalityCode { get; set; }
        public string? Active { get; set; }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
        public string? NationalityGUID { get; set; }
    }
    public class DeleteNationality
    {
        public DataTable NationalityDeleteTable = new DataTable();

        public DataTable ConvertToDataTable(List<DeleteNationalityList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteNationalityList).GetProperties();

            foreach (var property in properties)
            {

                NationalityDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = NationalityDeleteTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                NationalityDeleteTable.Rows.Add(row);
            }

            return NationalityDeleteTable;
        }


        public List<DeleteNationalityList>? DeleteDataTable { get; set; }

    }
    public class DeleteNationalityList
    {
        public string? NationalityGuid { get; set; }

    }
    public class DeleteNationalityResult
    {
        public long? SNo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
        public string? NationalityName { get; set; }
    }


}