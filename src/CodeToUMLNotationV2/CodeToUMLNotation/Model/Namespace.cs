//using CodeToUMLNotation.Model.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CodeToUMLNotation.Model
//{
//    public class Namespace : IUmlDesignation
//    {
//        public ICollection<CLRType> Types { get; private set; }

//        public Namespace()
//        {
//            Types = new LinkedList<CLRType>();
//        }

//        public void AppendSuffix(IRichStringbuilder richSb)
//        {
//            Types.Aggregate(new StringBuilder(), (sb, t) => sb.AppendLine(t.Design())).ToString();
//        }
//    }
//}
