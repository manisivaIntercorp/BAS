using DataAccessLayer.Interface;

namespace DataAccessLayer.Uow.Interface
{
    public interface IUowForgotPassword: IDisposable
    {
        IForgotPasswordDAL ForgotPasswordDALRepo { get; }
        void Commit();
    }
}
