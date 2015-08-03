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
    public class CLRType : Code, IUmlDesignation
    {
        public string BaseName { get; private set; }

        public CLRAvailableTypeMode Type { get; private set; }

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


        public string Design()
        {
            StringBuilder sb = new StringBuilder();
            string str = ":" + Name;

            Func<string> GetCLRTypeDescription = () =>
            {
                switch (Type)
                {
                    case CLRAvailableTypeMode.@class:

                        if (Abstract)
                        {
                            str += " « abstract »";
                        }
                        return str;

                    case CLRAvailableTypeMode.@enum:
                        return ":" + Name + " « enum »";

                    case CLRAvailableTypeMode.@interface:
                        return ":" + Name + " « interface »";

                    case CLRAvailableTypeMode.@struct:
                        return ":" + Name + " « struct »";

                    default: throw new NotSupportedException();
                }
            };

            string header = GetCLRTypeDescription();
            sb.Append(header);
            if (!string.IsNullOrEmpty(BaseName))
                sb.AppendLine(" extends " + BaseName);

            int numCharsOthersSeparators = sb.Length;
            int numCharsClass = (int)Math.Round((sb.Length * 1.4));        // fixed value.


            TryAddLineAtEnd(sb);

            sb.Append("`");
            

            // bool to indicate if you want a new line
            DrawLine(sb, numCharsClass, true, '-', false);
            sb.AppendLine("´");

            string references = GetReferencesUML();
            if (!string.IsNullOrEmpty(references))
            {
                DrawLine(sb, numCharsOthersSeparators, true, '-');
                sb.Append(references);
                DrawLine(sb, numCharsOthersSeparators, true, '-');
            }

            string constfields = GetConstantFieldsUML();
            if (!string.IsNullOrEmpty(constfields))
            {
                sb.Append("CONSTANTS ");
                DrawLine(sb, numCharsOthersSeparators, false, '#');
                sb.AppendLine(constfields);
            }

            string fields = GetFieldsUML();
            if (!string.IsNullOrEmpty(fields))
            {
                sb.Append("Fields ");
                DrawLine(sb, numCharsOthersSeparators, false, '#');
                sb.AppendLine(fields);
            }

            string properties = GetPropertiesUML();
            if( !string.IsNullOrEmpty(properties) ) {
                sb.Append("Properties ");
                DrawLine(sb, numCharsOthersSeparators, false, '#');
                sb.AppendLine(properties);
            }

            string methods = GetMethodsUML();

            if (!string.IsNullOrEmpty(methods))
            {
                sb.Append("Methods ");
                DrawLine(sb, numCharsOthersSeparators, false, '#');
                sb.AppendLine(methods);
            }

            return sb.ToString();
        }

        public string GetReferencesUML()
        {
            return ReferencedTypes.Aggregate(new StringBuilder(), (_, rt) => _.AppendLine("references: " + rt)).ToString();
        }


        public string GetConstantFieldsUML()
        {
            return ConstantFields.Aggregate(new StringBuilder(), (sb2, f) => sb2.AppendLine(f.Design())).ToString();
        }

        public string GetFieldsUML()
        {
            return Fields.Aggregate(new StringBuilder(), (sb2, f) => sb2.AppendLine(f.Design())).ToString();
        }

        public string GetPropertiesUML()
        {
            return Properties.Aggregate(new StringBuilder(), (sb2, f) => sb2.AppendLine(f.Design())).ToString();
        }

        public string GetMethodsUML()
        {
            return Methods.Aggregate(new StringBuilder(), (sb2, f) => sb2.AppendLine(f.Design())).ToString();
        }







        private void DrawLine(StringBuilder builder, int size, bool useIntercalatedSpace, char c, bool appendLineAtTheEnd = true)
        {
            if (useIntercalatedSpace)
            {
                for (int i = 0; i < size; builder.Append(i % 2 == 0 ? "" + c : " "), i++) ;
            }

            else
            {
                for (int i = 0; i < size; builder.Append(c), i++) ;
            }

            if (appendLineAtTheEnd)
                TryAddLineAtEnd(builder);
        }



        /// <summary>
        ///     Try to add another line at the end, if the last character is not already a new line
        /// </summary>
        /// <param name="sb"></param>
        private static void TryAddLineAtEnd(StringBuilder sb)
        {
            if (sb.Length == 0) return;

            if (sb[sb.Length - 1] != '\n')
                sb.AppendLine();                // doble confirmation to add another line
        }

    }
}

