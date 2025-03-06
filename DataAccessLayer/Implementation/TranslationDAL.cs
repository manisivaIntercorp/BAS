using Dapper;
using System.Data;
using System.Threading.Tasks;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Transactions;

namespace DataAccessLayer.Implementation
{
    public class TranslationDAL : ITranslationDAL
    {
        private readonly IDbConnection _connection;

        public TranslationDAL(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }



        public async Task<List<TranslationModel>> GetAllTranslations()
        {
            string storedProcedure = "sp_Transulation";
            return (await _connection.QueryAsync<TranslationModel>(
                storedProcedure,
                commandType: CommandType.StoredProcedure
            )).ToList();
        }
        //public async Task<string?> GetTranslation(string Name, string languageCode)
        //{
        //    string query = "SELECT Value FROM Translations WHERE Name = @Name AND LanguageCode = @LanguageCode";
        //    return await _connection.QueryFirstOrDefaultAsync<string>(query, new { Name = Name, LanguageCode = languageCode });
        //}
        //public async Task<string?> GetTranslation(string key, string languageCode, string scope)
        //{
        //    string query = "SELECT Value FROM Translations WHERE Key = @Key AND LanguageCode = @LanguageCode AND Scope = @Scope";
        //    return await _connection.QueryFirstOrDefaultAsync<string>(query, new { Key = key, LanguageCode = languageCode, Scope = scope });
        //}
        public async Task<bool> InsertOrUpdateTranslation(TranslationModel translation)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Key", translation.Key, DbType.String);
            parameters.Add("@LanguageCode", translation.LanguageCode, DbType.String);
            parameters.Add("@Value", translation.Value, DbType.String);

            int affectedRows = await _connection.ExecuteAsync("InsertOrUpdateTranslation", parameters, commandType: CommandType.StoredProcedure);

            return affectedRows > 0;
        }

        public async Task<List<Translation>> ExportResourceFiles(string ResourceName, string? Culture)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ResourceName", ResourceName, DbType.String);
            parameters.Add("@Culture", Culture, DbType.String);
            string storedProcedure = "sp_ExportTranslation";

            // FIX: Remove List<> from QueryAsync
            return (await _connection.QueryAsync<Translation>(
                storedProcedure,
                parameters, // Add parameters
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<bool> InsertOrUpdateTranslationList(List<Translation> translation)
        {
            var table = new DataTable();
            table.Columns.Add("ResourceName", typeof(string));
            table.Columns.Add("TranslationKey", typeof(string));
            table.Columns.Add("LanguageCode", typeof(string));
            table.Columns.Add("TranslationValue", typeof(string));

            foreach (var record in translation)
            {
                table.Rows.Add(
                    record.ResourceName ?? (object)DBNull.Value,
                    record.TranslationKey ?? (object)DBNull.Value,
                    record.LanguageCode ?? (object)DBNull.Value,
                    record.TranslationValue ?? (object)DBNull.Value
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Translations", table.AsTableValuedParameter("utt_LanguageTranslation"));
            parameters.Add("@AffectedRows", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync("sp_ImportTranslations", parameters, commandType: CommandType.StoredProcedure);

            int affectedRows = parameters.Get<int>("@AffectedRows");

            return affectedRows > 0; // Corrected
        }




    }
}
