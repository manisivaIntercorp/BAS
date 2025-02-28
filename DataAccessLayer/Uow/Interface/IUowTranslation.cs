using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Uow.Interface
{
    public interface IUowTranslation
    {
        ITranslationDAL TranslationDALRepo { get; }
        void Commit();

        Task<List<TranslationModel>> GetAllTranslations();

        // Add the missing method
        //Task<string?> GetTranslation(string key, string languageCode, string scope);
    }
}
