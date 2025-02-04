using DataAccessLayer.Model;
using DataAccessLayer.Services;

namespace DataAccessLayer.Interface
{
    public interface IForgotPasswordDAL
    {
        Task<(List<ForgotPasswordModel> forgotPasswordModels, int RetVal, string Msg)> InsertUpdateForgotPassword(ForgotPasswordModel LM);
        MailServer mailServerport();
        Task<GetForgotPasswordModel?> ValidateTokenForgotPassword(string? token);
    }
}
