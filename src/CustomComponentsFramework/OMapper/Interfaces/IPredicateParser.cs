using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace OMapper.Interfaces
{
    public interface IPredicateParser
    {
        String ParseFilter(Expression expr);
    }
}
