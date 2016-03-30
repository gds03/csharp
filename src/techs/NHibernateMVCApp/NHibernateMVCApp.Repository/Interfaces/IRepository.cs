using NHibernateMVCApp.Repository.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateMVCApp.Repository.Interfaces
{
    public interface IRepository
    {
        Query<TValue> Query<TValue>() where TValue : new();
        TValue Insert<TValue>(TValue @object)   where TValue : new();
        void Update<TValue>(TValue @object) where TValue : new();
        void Delete<TValue>(TValue @object) where TValue : new();

        TValue[] GetAll<TValue>()               where TValue : new();
        TValue GetById<TValue, TId>(TId id)     where TValue : new()
                                                where TId : struct;
    }


   
}
