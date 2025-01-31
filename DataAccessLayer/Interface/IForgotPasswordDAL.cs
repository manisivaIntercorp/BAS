

using DataAccessLayer.Implementation;
using DataAccessLayer.Model;

namespace DataAccessLayer.Interface
{
    public interface IForgotPasswordDAL
    {
        Task<(List<ForgotPasswordModel> forgotPasswordModels, int RetVal, string Msg)> InsertUpdateForgotPassword(ForgotPasswordModel LM);
        MailServer mailServerport();
        

    }
}
