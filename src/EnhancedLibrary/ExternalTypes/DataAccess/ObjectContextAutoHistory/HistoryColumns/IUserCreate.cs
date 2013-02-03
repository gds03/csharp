using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnhancedLibrary.ExternalTypes.DataAccess.ObjectContextAutoHistory.HistoryColumns
{
    public interface IUserCreate<T> where T : struct
    {
        T UserCria { get; set; }
        DateTime DataCria { get; set; }
    }
}
