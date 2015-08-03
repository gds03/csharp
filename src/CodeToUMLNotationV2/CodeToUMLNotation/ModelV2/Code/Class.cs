using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Code
{
    public class @Class : ClassesAndStructs, IUmlDesign, ITypeDeclaration
    {
        const String ABSTRACT_WORD = "« abstract »";
        const String STATIC_WORD = "[STATIC]";


        public bool Static { get; private set; }

        public bool Abstract { get; private set; }

        public Class(Visibility visibility, String name,
            bool @static, bool @abstract) : base(visibility, name)
        {
            Static = @static;
            Abstract = @abstract;
        }

        protected override int DesignHeaderConcrete(IRichStringbuilder richSb)
        {
            int charsWritten = 0;
            if (Abstract)
            {
                richSb.WriteRegular(" ");
                richSb.WriteItalic(ABSTRACT_WORD);
                charsWritten += ABSTRACT_WORD.Length;
            }

            if (Static)
            {
                richSb.WriteRegular(" ");
                richSb.WriteItalic(STATIC_WORD);
                charsWritten += STATIC_WORD.Length;
            }

            return charsWritten;
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
