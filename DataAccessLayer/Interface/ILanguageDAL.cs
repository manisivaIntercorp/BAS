using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ILanguageDAL
    {
        Task<List<LanguageModel>> GetAllLanguage();
        Task<List<LanguageNameEnum>> GetAllLanguageinDropdown();

        Task<bool> InsertUpdateLanguage(LanguageModel LM);
        
        Task<bool> DeleteLanguage(int Id);
        Task<LanguageModel> GetLanguageById(int Id);
        Task<bool> UpdateLanguageAsync(int id, LanguageModel objModel);
    }
}
