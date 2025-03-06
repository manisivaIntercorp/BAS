using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ITranslationDAL
    {
      //Task<string?> GetTranslation(string key, string languageCode);
      //  Task<bool> InsertOrUpdateTranslation(TranslationModel translation);
        Task<bool> InsertOrUpdateTranslationList(List<Translation> translation);
                   
        Task<List<TranslationModel>> GetAllTranslations();
        Task<List<Translation>> ExportResourceFiles(string ResourceName, string? Culture);

        //Task<string?> GetTranslation(string key, string languageCode, string scope); // Updated method signature

    }
}
