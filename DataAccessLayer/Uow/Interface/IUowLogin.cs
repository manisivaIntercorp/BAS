﻿using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Uow.Interface
{
    public interface IUowLogin:IDisposable
    {
        ILoginDAL LoginDALRepo { get; }
        void commit();
    }
}
