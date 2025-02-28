using DataAccessLayer.Model;


namespace DataAccessLayer.Interface
{
    public interface INationalityDAL
    {
        Task<List<GetNationalityModel>> GetAllNationality();
        Task<(bool Insertnationality, long RetVal, string Msg)> InsertUpdateNationality(NationalityModel NM);
        Task<(bool Updatenationality, long RetVal, string Msg)> UpdateNationality(UpdateNationality NM);
        Task<(bool deletenationality, List<DeleteNationalityResult> deleteResults)> DeleteNationality(long Id, DeleteNationality deleteNationality);
        Task<GetNationalityModel?> GetNationalityByGUId(string Id);

    }
}
