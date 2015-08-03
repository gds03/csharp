using CodeToUMLNotation.Model.Enums;
using CodeToUMLNotation.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model
{
    public class Visibility : IUmlDesignation
    {
        public VisibilityMode Mode { get; private set; }

        public Visibility(VisibilityMode visibility)
        {
            Mode = visibility;
        }

        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            Func<string> f = () => {
                switch (Mode)
                {
                    case VisibilityMode.@internal:
                        return "@";
                    case VisibilityMode.@private:
                        return "-";
                    case VisibilityMode.@protected: 
                        return "#";
                    case VisibilityMode.@public:
                        return "+";

                    default:
                        throw new NotSupportedException();
                }
            };

            string symbol = f();
            richSb.WriteRegular(symbol);
            return richSb;
        }
    }
}
