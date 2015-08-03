using CodeToUMLNotation.ModelV2.Code;
using CodeToUMLNotation.ModelV2.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Abstract
{
    public abstract class Declaration
    {
        public Visibility Visibility { get; private set; }

        public string Name { get; private set; }

        public Declaration(Visibility visibility, String name)
        {
            Name = name;
            Visibility = visibility;
        }


        #region Protected Helpers

        protected static void DrawLine(IRichStringbuilder builder, int size, bool useIntercalatedSpace, char c, bool appendLineAtTheEnd = true, bool writingHeader = false)
        {
            Action<bool, string> internalFunc = (header, data) =>
            {
                if (header) { builder.WriteBold(data); }
                else { builder.WriteRegular(data); }
            };

            if (useIntercalatedSpace)
            {
                for (int i = 0;
                     i < size;
                     internalFunc(writingHeader, (i % 2 == 0 ? "" + c : " ")), i++) ;
            }

            else
            {
                for (int i = 0;
                     i < size;
                     internalFunc(writingHeader, ("" + c)), i++) ;
            }

            if (appendLineAtTheEnd)
            {
                builder.WriteLine();
            }
        }

        #endregion
    }
}
