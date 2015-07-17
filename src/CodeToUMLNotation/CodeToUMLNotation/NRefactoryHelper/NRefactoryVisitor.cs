using CodeToUMLNotation.Model;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.NRefactoryHelper
{
    public class NRefactoryVisitor : DepthFirstAstVisitor
    {
        public IEnumerable<CLRType> CLRTypes {
            get
            {
                return m_clrTypes.Value.ToList();
            }
        }


        private readonly Lazy<LinkedList<CLRType>> m_clrTypes;        
        
        

        public NRefactoryVisitor()
        {
            m_clrTypes = new Lazy<LinkedList<CLRType>>(() => new LinkedList<CLRType>());            
        }

        public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
        {
            CLRType t = m_clrTypes.Value.Last.Value;
            string returnType =  fieldDeclaration.ReturnType.ToString();

            Field f = new Field(
                 CheckFlag(fieldDeclaration.Modifiers, Modifiers.Static) ,
                 CheckFlag(fieldDeclaration.Modifiers, Modifiers.Readonly),
                 new Visibility(VisibilityMapper.Map(fieldDeclaration.Modifiers)),
                 CheckFlag(fieldDeclaration.Modifiers, Modifiers.Virtual) ,
                 fieldDeclaration.Variables.Single().Name,
                 CheckFlag(fieldDeclaration.Modifiers, Modifiers.Abstract) ,
                 returnType
            );

            t.Fields.Add(f);    // connect
            AddToNotDefaultReferencedTypes(returnType);

            // call base to forward execution
            base.VisitFieldDeclaration(fieldDeclaration);
        }


        public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
        {
            CLRType t = m_clrTypes.Value.Last.Value;
            string returnType = propertyDeclaration.ReturnType.ToString();

            Property p = new Property(
                CheckFlag(propertyDeclaration.Modifiers, Modifiers.Override) ,
                CheckFlag(propertyDeclaration.Modifiers , Modifiers.Static),
                new Visibility(VisibilityMapper.Map(propertyDeclaration.Modifiers)),
                CheckFlag(propertyDeclaration.Modifiers , Modifiers.Virtual),
                propertyDeclaration.Name,
                CheckFlag(propertyDeclaration.Modifiers, Modifiers.Abstract),
                propertyDeclaration.ReturnType.ToString(),
                propertyDeclaration.Getter != null,
                propertyDeclaration.Setter != null
                );


            t.Properties.Add(p);    // connect
            AddToNotDefaultReferencedTypes(returnType);

            // call base to forward execution
            base.VisitPropertyDeclaration(propertyDeclaration);
        }


        public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
        {
            AddMethod(true, constructorDeclaration, constructorDeclaration.Parameters);

            // call base to forward execution
            base.VisitConstructorDeclaration(constructorDeclaration);
        }

        public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            AddMethod(false, methodDeclaration, methodDeclaration.Parameters);
            string returnType = methodDeclaration.ReturnType.ToString();
            AddToNotDefaultReferencedTypes(returnType);

            // call base to forward execution
            base.VisitMethodDeclaration(methodDeclaration);
        }



        public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            CLRType t = new CLRType(
                CheckFlag(typeDeclaration.Modifiers, Modifiers.Static ),
                new Visibility(VisibilityMapper.Map(typeDeclaration.Modifiers)),
                CheckFlag(typeDeclaration.Modifiers , Modifiers.Virtual),
                typeDeclaration.Name,
                CheckFlag(typeDeclaration.Modifiers, Modifiers.Abstract),
                CLRAvailableTypeModeMapper.Map(typeDeclaration.ClassType),
                GetBaseTypesForTypeDeclaration(typeDeclaration)
            );

            m_clrTypes.Value.AddLast(t);


            // call base to forward execution
            base.VisitTypeDeclaration(typeDeclaration);
        }



        #region Helpers

        private bool AddToNotDefaultReferencedTypes(string type)
        {
            ParameterValidator.ThrowIfArgumentNullOrEmpty(type, "type");
            IEnumerable<string> systemTypes = typeof(bool).Assembly.GetTypes().Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.StartsWith("System")).Select(x => x.Name.ToLower());
            string typeLower = type.ToLower();

            typeLower = AdjustTypeMismatch(typeLower);

            if (!m_clrTypes.Value.Last.Value.ReferencedTypes.Contains(type) && !systemTypes.Contains(typeLower))
            {
                m_clrTypes.Value.Last.Value.ReferencedTypes.Add(type);
                return true;
            }
            return false;
        }

        private static string AdjustTypeMismatch(string typeLower)
        {
            ParameterValidator.ThrowIfArgumentNullOrEmpty(typeLower, "typeLower");

            if (typeLower.Last() == '?')
                typeLower = typeLower.Substring(0, typeLower.Length - 1);

            if (typeLower == "bool")
                typeLower = "boolean";
            
            if (typeLower == "int")
                typeLower = "int32";

            return typeLower;
        }

        private static string GetBaseTypesForTypeDeclaration(TypeDeclaration td)
        {
            ParameterValidator.ThrowIfArgumentNull(td, "td");
            StringBuilder data = new StringBuilder();

            foreach(var i in td.BaseTypes)
            {
                SimpleType st = i as SimpleType;
                if (st != null)
                {
                    data.Append(st.IdentifierToken.Name + ", ");
                }
            }

            if (data.Length > 0)
                data.Remove(data.Length - 2, 2);

            return data.ToString();
        }



        private void AddMethod(bool ctor, EntityDeclaration methodDeclaration, AstNodeCollection<ParameterDeclaration> parameters)
        {
            CLRType t = m_clrTypes.Value.Last.Value;

            List<KeyValuePair<string, string>> args = parameters.Select(p =>
                                                        new KeyValuePair<string, string>(p.Name, p.Type.ToString())
                                                      )
                                                      .ToList();


            Method m = new Method(
                CheckFlag(methodDeclaration.Modifiers, Modifiers.Override),
                ctor,
                CheckFlag(methodDeclaration.Modifiers, Modifiers.Static),
                new Visibility(VisibilityMapper.Map(methodDeclaration.Modifiers)),
                CheckFlag(methodDeclaration.Modifiers, Modifiers.Virtual),
                methodDeclaration.Name,
                CheckFlag(methodDeclaration.Modifiers, Modifiers.Abstract),
                args,
                methodDeclaration.ReturnType.ToString()

            );

            t.Methods.Add(m);   // connect
        }

        private static bool CheckFlag(Modifiers modifiers, Modifiers current)
        {
            return (modifiers & current) == current;
        }

        #endregion

    }
}
