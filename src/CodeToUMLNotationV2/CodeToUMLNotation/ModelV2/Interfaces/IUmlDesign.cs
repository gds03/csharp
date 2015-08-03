using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Interfaces
{
    public interface IUmlDesign
    {
        IRichStringbuilder Design(IRichStringbuilder richSb);
    }
}
