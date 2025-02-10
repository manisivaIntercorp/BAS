using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface INationalityDAL
    {
        Task<List<NationalityModel>> GetAllNationality();
        Task<(bool Insertnationality, int RetVal, string Msg)> InsertUpdateNationality(NationalityModel NM);
        Task<(bool Updatenationality, int RetVal, string Msg)> UpdateNationality(NationalityModel NM);
        Task<bool> DeleteNationality(int Id);
        Task<NationalityModel> GetNationalityById(int Id);

    }
}
