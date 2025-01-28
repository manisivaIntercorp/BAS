using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Uow.Interface
{
    /// <summary>
    /// Interface for Unit of Work specific to Organisation operations.
    /// Manages transactions and repositories for Organisation data access.
    /// </summary>
    public interface IUowOrganisation : IDisposable
    {
        /// <summary>
        /// Gets the Organisation Data Access Repository.
        /// </summary>
        IOrganisationDAL OrganisationDALRepo { get; }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        void Commit();
    }
}



