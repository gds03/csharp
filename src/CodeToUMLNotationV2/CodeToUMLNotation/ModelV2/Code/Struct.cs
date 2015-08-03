using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Code
{
    public class Struct : ClassesAndStructs, IUmlDesign, ITypeDeclaration
    {
        const string STRUCT_WORD = "« struct »";

        public Struct(Visibility visibility, String name) : base(visibility, name)
        {

        }

        protected override int DesignHeaderConcrete(IRichStringbuilder richSb)
        {
            richSb.WriteRegular(" ");
            richSb.WriteItalic(STRUCT_WORD);
            return STRUCT_WORD.Length;
        }

        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            // Header
            DesignHeader(richSb);

            // REFERENCES
            if (ReferencedTypes.Any())
            {
                richSb.WriteLine();
                DrawLine(richSb, 50, true, '-');
                WriteReferencesUML(richSb);
                DrawLine(richSb, 50, true, '-');
            }

            // CONSTANTS
            if (ConstantFields.Any())
            {
                richSb.WriteLine();
                richSb.WriteRegular("CONSTANTS ").WriteLine();
                WriteConstantsUML(richSb);
            }
            // FIELDS
            if (Fields.Any())
            {
                richSb.WriteLine();
                richSb.WriteRegular("Fields ").WriteLine();
                WriteFieldsUML(richSb);
            }

            // PROPERTIES
            if (Properties.Any())
            {
                richSb.WriteLine();
                richSb.WriteRegular("Properties ").WriteLine();
                WritePropertiesUML(richSb);
            }

            // METHODS
            if (Methods.Any())
            {
                richSb.WriteLine();
                richSb.WriteRegular("Methods ").WriteLine();
                WriteMethodsUML(richSb);
            }

            return richSb;
        }



        public void DesignType(IRichStringbuilder richSb)
        {
            Design(richSb);
        }
    }
}
