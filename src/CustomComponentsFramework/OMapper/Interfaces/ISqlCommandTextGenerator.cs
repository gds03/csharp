using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace OMapper.Interfaces
{
    public interface ISqlCommandTextGenerator
    {
        string SelectCommand<T>(Expression<Func<T, bool>> predicate) where T : class;

        string UpdateCommand(object obj, string[] objPropertiesToUpdate);

        string InsertCommand(object obj);

        string DeleteCommand(object obj);

    }
}
