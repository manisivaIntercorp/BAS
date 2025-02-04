using System.Data;
using DataAccessLayer.Services;

namespace DataAccessLayer.Model
{
    public class ForgotPasswordModel
    {
        public int? ID
        {
            get;
            set;
        }

        public string? UserName
        {
            get;
            set;
        }
        DataTable Emailtable = new DataTable();
        DataTable MailServerTable = new DataTable();

        public DataTable ConvertToDataTableAsync(List<GetEmailTemplate?> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(GetEmailTemplate).GetProperties();
            foreach (var property in properties)
            {
                Emailtable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = Emailtable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                Emailtable.Rows.Add(row);
            }
            return Emailtable;
        }

        public DataTable ConvertToDataTableAsync(List<MailServer?> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(MailServer).GetProperties();
            foreach (var property in properties)
            {
                MailServerTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = MailServerTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                MailServerTable.Rows.Add(row);
            }
            return MailServerTable;
        }
        public string Token { get; } = Guid.NewGuid().ToString("N"); //Auto Generated Token
    }
    public class ForgotPasswordRequest
    {
        public ForgotPasswordModel ForgotPasswordModel { get; set; }
        public EmailTemplate _emailrepository { get; set; }
        public ForgotPasswordRequest()
        {
            _emailrepository = new EmailTemplate(); // Ensure it's never null
            ForgotPasswordModel = new ForgotPasswordModel();
        }
    }
    public class GetForgotPasswordModel
    {
        public long? UserID { get; set; }
        public string? UserName { get; set; }
        public long? ID { get; set; }
    }
}


