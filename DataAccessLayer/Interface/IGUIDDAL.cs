

using DataAccessLayer.Implementation;
using DataAccessLayer.Services;

namespace DataAccessLayer.Interface
{
    public interface IGUIDDAL
    {
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUID(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnUserAccountRoleGuid(string UserGuid);
        Task<(bool GetGuid, int RetVal, string Msg)> GetGUIDBasedOnRoleGuid(string UserGuid);

    }
}
