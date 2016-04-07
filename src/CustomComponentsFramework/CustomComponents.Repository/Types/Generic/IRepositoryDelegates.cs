using CustomComponents.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Repository.Types.Generic
{
    public delegate void Callback(IRepository r);
    public delegate TResult CallbackResult<TResult>(IRepository r);
    public delegate void ExceptionCallback(Exception ex);
}
