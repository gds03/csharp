using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.WinForms.Types.AutoCallbackServiceAgent.Generic
{
    public sealed class CallbackResult<TResult>
    {
        readonly Exception m_Exception;
        readonly TResult m_Result;

        public CallbackResult(TResult result)
        {
            this.m_Result = result;
        }

        public CallbackResult(Exception exception)
        {
            this.m_Exception = exception;
        }

        public CallbackResult(TResult result, Exception exception)
            : this(result)
        {
            this.m_Exception = exception;
        }

        public Exception Exception { get { return m_Exception; } }


        public TResult Result { get { return m_Result; } }
    }

}
