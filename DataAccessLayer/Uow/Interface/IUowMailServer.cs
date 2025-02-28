using System;
using DataAccessLayer.Interface;


namespace DataAccessLayer.Uow.Interface
{
    public interface IUowMailServer : IDisposable
    {
        IMailServerDAL MailServerDALRepo { get; }
        void Commit();
    }
}
