using CodeToUMLNotation.ModelV2;
using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.NRefactoryHelper.Interfaces;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using al = CodeToUMLNotation.NRefactoryHelper.Interfaces.NRefactoryVisitorV2DeclarationHelper;

namespace CodeToUMLNotation.NRefactoryHelper
{
    public class NRefactoryVisitorV2 : DepthFirstAstVisitor
    {
        public IEnumerable<Declaration> Declarations { 
            get { return m_declarations.Value.ToList(); }
        }


        private readonly Lazy<LinkedList<Declaration>> m_declarations = new Lazy<LinkedList<Declaration>>(() => new LinkedList<Declaration>());    

        // Fields there is only on Classes or Structs
        public override void VisitFieldDeclaration(FieldDeclaration fd)
        {
            ClassesAndStructs cs = GetLastTwithinTheList<ClassesAndStructs>();
            
            string name = fd.Variables.Single().Name;
            string returnType = fd.ReturnType.ToString();
            Visibility v = new Visibility(VisibilityMapper.Map(fd.Modifiers));

            if (al.CheckFlag(fd.Modifiers, Modifiers.Const))
            {
                Constant cf = new Constant(v, name, returnType, fd.Variables.Single().Initializer.ToString());
                cs.ConstantFields.Add(cf);
            }

            else
            {
                Field f = new Field(v, name,
                     al.CheckFlag(fd.Modifiers, Modifiers.Static),
                     al.CheckFlag(fd.Modifiers, Modifiers.Readonly),
                     returnType
                );

                cs.Fields.Add(f);
            }
            AddToNotDefaultReferencedTypes(cs, returnType);

            // call base to forward execution
            base.VisitFieldDeclaration(fd);
        }

        // Properties there is only on Classes or Structs or Interfaces
        public override void VisitPropertyDeclaration(PropertyDeclaration pd)
        {
            ClassesAndStructsAndInterfaces csi = GetLastTwithinTheList<ClassesAndStructsAndInterfaces>();
            string returnType = pd.ReturnType.ToString();

            Property p = new Property(
                new Visibility(VisibilityMapper.Map(pd.Modifiers)),
                pd.Name,
                al.CheckFlag(pd.Modifiers, Modifiers.Static),
                al.CheckFlag(pd.Modifiers, Modifiers.Virtual),
                al.CheckFlag(pd.Modifiers, Modifiers.Abstract),
                al.CheckFlag(pd.Modifiers, Modifiers.Override),
                returnType,
                pd.Getter.HasChildren,
                pd.Setter.HasChildren
            );


            csi.Properties.Add(p);
            AddToNotDefaultReferencedTypes(csi, returnType);

            // call base to forward execution
            base.VisitPropertyDeclaration(pd);
        }

        // Constructors there is only on Classes or Structs
        public override void VisitConstructorDeclaration(ConstructorDeclaration cd)
        {
            AddMethod(true, cd, cd.Parameters);

            // call base to forward execution
            base.VisitConstructorDeclaration(cd);
        }

        // Methods there is only on Classes or Structs or Interfaces
        public override void VisitMethodDeclaration(MethodDeclaration md)
        {
            ClassesAndStructsAndInterfaces csi = GetLastTwithinTheList<ClassesAndStructsAndInterfaces>();
            AddMethod(false, md, md.Parameters);
            string returnType = md.ReturnType.ToString();
            AddToNotDefaultReferencedTypes(csi, returnType);

            // call base to forward execution
            base.VisitMethodDeclaration(md);
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclaration ed)
        {
            ModelV2.Code.Enum e = GetLastTwithinTheList<ModelV2.Code.Enum>();

            string[] keyValue = ed.GetText().Split('=');
            e.Values.Add(new KeyValuePair<string,string>(keyValue[0], (keyValue.Length == 2) ? keyValue[1] : ""));

            // call base
            base.VisitEnumMemberDeclaration(ed);
        }

        public override void VisitTypeDeclaration(TypeDeclaration td)
        {
            Declaration d = NRefactoryVisitorV2DeclarationHelper.Converters.Single(x => x.Handle(td.ClassType)).Create(td);
            m_declarations.Value.AddLast(d);

            // call base
            base.VisitTypeDeclaration(td);
        }



        #region Helpers


        private T GetLastTwithinTheList<T>() where T : Declaration
        {
            if (m_declarations.IsValueCreated == false)
                return null;

            return m_declarations.Value.OfType<T>().Last();            
        }



        private static bool AddToNotDefaultReferencedTypes(ClassesAndStructsAndInterfaces csi, string type)
        {
            ParameterValidator.ThrowIfArgumentNullOrEmpty(type, "type");
            IEnumerable<string> systemTypes = typeof(bool).Assembly.GetTypes().Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.StartsWith("System")).Select(x => x.Name.ToLower());
            string typeLower = type.ToLower();

            typeLower = AdjustTypeMismatch(typeLower);

            if (!csi.ReferencedTypes.Contains(type) && !systemTypes.Contains(typeLower))
            {
                csi.ReferencedTypes.Add(type);
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

        



        private void AddMethod(bool ctor, EntityDeclaration md, AstNodeCollection<ParameterDeclaration> ps)
        {
            ClassesAndStructsAndInterfaces csi = (ClassesAndStructsAndInterfaces) m_declarations.Value.Last.Value;

            List<KeyValuePair<string, string>> args = ps.Select(p =>
                                                        new KeyValuePair<string, string>(p.Name, p.Type.ToString())
                                                      )
                                                      .ToList();


            Method m = new Method(
                new Visibility(VisibilityMapper.Map(md.Modifiers)),
                md.Name,
                al.CheckFlag(md.Modifiers, Modifiers.Static),
                al.CheckFlag(md.Modifiers, Modifiers.Virtual),
                al.CheckFlag(md.Modifiers, Modifiers.Abstract),
                al.CheckFlag(md.Modifiers, Modifiers.Override),
                md.ReturnType.ToString(),
                ctor,                
                args
            );

            csi.Methods.Add(m);
        }


        #endregion

    }
}
