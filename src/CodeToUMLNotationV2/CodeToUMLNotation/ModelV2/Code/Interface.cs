using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Code
{
    public class Interface : ClassesAndStructsAndInterfaces, IUmlDesign, ITypeDeclaration
    {
        const String INTERFACE_WORD = "« interface »";
        public Interface(Visibility visibility, String name) : base(visibility, name)
        {
            if (visibility.Mode == Model.Enums.VisibilityMode.@private || visibility.Mode == Model.Enums.VisibilityMode.@protected)
                throw new InvalidOperationException("Interface can only be public or internal");
        }

        protected override int DesignHeaderConcrete(IRichStringbuilder richSb)
        {
            richSb.WriteRegular(" ");
            richSb.WriteItalic(INTERFACE_WORD);
            return INTERFACE_WORD.Length;
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
