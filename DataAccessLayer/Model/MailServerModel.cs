using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;


namespace DataAccessLayer.Model
{
    public class MailServerModel
    {
        [JsonIgnore]
        public long? Id {  get; set; }
        [EmailAddress]
        public string? ReplyEMail { get; set; }
        public string? SMTP_Address {  get; set; }
        public string? CredentialRequired {  get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SMTP_Port {  get; set; }
        public string? SSL_Required { get; set; }
        [JsonIgnore]
        public long? CreatedBy {  get; set; }
        public string? Active {  get; set; }
        public string? SetupName {  get; set; }
        public string? IsDefault {  get; set; }
        
    }
    public class GetMailServerModel
    {
        
        public long? Id { get; set; }
        public string? ReplyEMail { get; set; }
        public string? SMTP_Address { get; set; }
        public string? CredentialRequired { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SMTP_Port { get; set; }
        public string? SSL_Required { get; set; }
        public string? CreatedBy { get; set; }
        public string? Active { get; set; }
        public string? SetupName { get; set; }
        public string? IsDefault { get; set; }
        
        public string? MailServerGuid { get; set; }

    }
    public class UpdateMailServerModel {
        [JsonIgnore]
        public long? Id { get; set; }
        [EmailAddress]
        public string? ReplyEMail { get; set; }
        public string? SMTP_Address { get; set; }
        public string? CredentialRequired { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SMTP_Port { get; set; }
        public string? SSL_Required { get; set; }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
        public string? Active { get; set; }
        public string? SetupName { get; set; }
        public string? IsDefault { get; set; }
        
        public string? MailServerGuid { get; set; }
    }
    public class DeleteMailServer
    {
        public DataTable MailServerDeleteTable = new DataTable();

        public DataTable ConvertToDataTable(List<DeleteMailServerList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteMailServerList).GetProperties();

            foreach (var property in properties)
            {

                MailServerDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = MailServerDeleteTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                MailServerDeleteTable.Rows.Add(row);
            }

            return MailServerDeleteTable;
        }


        public List<DeleteMailServerList>? DeleteDataTable { get; set; }

    }
    public class DeleteMailServerList
    {
        public string? MailServerGuid { get; set; }

    }
    public class DeleteMailServerResult
    {
        public long? SNo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
        public string? SetupName { get; set; }
    }
}
