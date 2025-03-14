using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IMenuDAL
    {
        Task<List<MenuModel>> GetAllMenu();
        Task<List<MenusModel>> GetMenu(string UserName, string ClientCode, int OrgID);
    }
}
