using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation
{
    public interface IRichStringbuilder
    {
        int Length { get; }

        IRichStringbuilder WriteBold(string text);
        IRichStringbuilder WriteItalic(string text);
        IRichStringbuilder WriteUnderline(string text);
        IRichStringbuilder WriteRegular(string text);

        IRichStringbuilder WriteLine();

        IRichStringbuilder DeleteLast(int numChars);
    }
}
