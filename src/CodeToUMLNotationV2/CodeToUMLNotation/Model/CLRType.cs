using CodeToUMLNotation.Model.Abstract;
using CodeToUMLNotation.Model.Const;
using CodeToUMLNotation.Model.Enum;
using CodeToUMLNotation.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model
{
    public class CLRType : Declaration, IUmlDesignation
    {
        public string BaseName { get; private set; }

        public CLRAvailableTypeMode Type { get; private set; }                  // type of this CLRType (interface, class, struct)
        public ICollection<ConstField> ConstantFields { get; private set; }
        public ICollection<Field> Fields { get; private set; }
        public ICollection<Property> Properties { get; private set; }
        public ICollection<Method> Methods { get; private set; }
        public ICollection<string> ReferencedTypes { get; private set; }

        public CLRType(bool @static, Visibility visibility, bool @virtual, string name, bool @abstract, CLRAvailableTypeMode _CLRType, string baseName) : base(@static, visibility, @virtual,name, @abstract)
        {
            ConstantFields = new LinkedList<ConstField>();

            Fields = new LinkedList<Field>();
            Properties = new LinkedList<Property>();
            Methods = new LinkedList<Method>();
            ReferencedTypes = new HashSet<String>();

            Type = _CLRType;
            BaseName = baseName;   
        }


        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            string dummyTextPreparation = Name;                  
            Func<string> annotationString = () =>
            {
                string s = string.Empty;
                switch (Type)
                {
                    case CLRAvailableTypeMode.@class:
                        if (Abstract) s = "« abstract »";
                        break;
                    case CLRAvailableTypeMode.@enum:
                        s = " « enum »";
                        break;

                    case CLRAvailableTypeMode.@interface:
                        s = " « interface »";
                        break;

                    case CLRAvailableTypeMode.@struct:
                        s = " « struct »";
                        break;

                    default: throw new NotSupportedException();
                }

                return s;
            };

            if (!string.IsNullOrEmpty(BaseName)) { dummyTextPreparation += " extends " + BaseName; }
            string annotationText = annotationString();
            dummyTextPreparation += annotationText;         // append

            // calc size
            int numCharsOthersSeparators = (int)Math.Round(dummyTextPreparation.Length * 0.8);
            int numCharsClass = (int)Math.Round((dummyTextPreparation.Length * 1.3));        // fixed value because of the caracter _

            DrawLine(richSb, numCharsClass, true, '_', true, true);            
            richSb.WriteRegular(": ");
            Visibility.Design(richSb);
            richSb.WriteRegular(" " + Name);

            if (!string.IsNullOrEmpty(annotationText)) { richSb.WriteItalic(annotationText); }
            if (!string.IsNullOrEmpty(BaseName)) { richSb.WriteRegular(" extends ").WriteBold(BaseName); }            

            richSb.WriteLine();
            DrawLine(richSb, numCharsClass, true, '¯', true, true);

            // REFERENCES
            if (ReferencedTypes.Any())
            {
                richSb.WriteLine();
                DrawLine(richSb, numCharsOthersSeparators, true, '-');
                WriteReferencesUML(richSb);
                DrawLine(richSb, numCharsOthersSeparators, true, '-');
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

        #region Public Helpers

        public bool WriteReferencesUML(IRichStringbuilder richSb)
        {
            ReferencedTypes.ToList().ForEach(x => richSb.WriteRegular("references: " + x).WriteLine());
            return ReferencedTypes.Count > 0;
        }


        public bool WriteConstantsUML(IRichStringbuilder richSb)
        {
            ConstantFields.ToList().ForEach(c => c.Design(richSb).WriteLine());
            return ConstantFields.Count > 0;
        }

        public bool WriteFieldsUML(IRichStringbuilder richSb)
        {
            Fields.ToList().ForEach(f => f.Design(richSb).WriteLine());
            return Fields.Count > 0;
        }

        public bool WritePropertiesUML(IRichStringbuilder richSb)
        {
            Properties.ToList().ForEach(p => p.Design(richSb).WriteLine());
            return Properties.Count > 0;
        }
        public bool WriteMethodsUML(IRichStringbuilder richSb)
        {
            Methods.ToList().ForEach(m => m.Design(richSb).WriteLine());
            return Methods.Count > 0;
        }

        #endregion



        #region Private Helpers

        private void DrawLine(IRichStringbuilder builder, int size, bool useIntercalatedSpace, char c, bool appendLineAtTheEnd = true, bool writingHeader = false)
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
                     internalFunc( writingHeader, (i % 2 == 0 ? "" + c : " ")), i++) ;
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

