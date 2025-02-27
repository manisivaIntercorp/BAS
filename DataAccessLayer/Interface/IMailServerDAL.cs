using DataAccessLayer.Model;
namespace DataAccessLayer.Interface
{
    public interface IMailServerDAL
    {
        Task<List<GetMailServerModel>> GetAllMailServer();
        Task<(bool Insertmailserver, long RetVal, string Msg)> InsertUpdateMailServer(MailServerModel NM);
        Task<(bool Updatemailserver, long RetVal, string Msg)> UpdateMailServer(UpdateMailServerModel NM);
        Task<(bool deletemailserver, List<DeleteMailServerResult> deleteResults)> DeleteMailServer(long Id, DeleteMailServer deleteNationality);
        Task<GetMailServerModel?> GetMailServerByGUId(string Id);
    }
}
