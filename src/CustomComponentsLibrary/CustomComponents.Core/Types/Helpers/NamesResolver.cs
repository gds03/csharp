using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.Types.Helpers
{
    #region Method Helper


    class MethodPathVisitor<TModel> : ExpressionVisitor
    {
        public MethodInfo Result { get; private set; }

        public MethodPathVisitor(Expression<Func<TModel, object>> expr)
        {
            Visit(expr);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (Result != null)
                throw new InvalidOperationException();

            Result = node.Method;
            return base.VisitMethodCall(node);
        }
    }


    #endregion







    #region Properties Helper


    public class PropertyResolverResult
    {
        /// <summary>
        ///     Get the full path to the final property
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        ///     Get the value of the final property if model was passed
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     Get the type of the final property.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        ///     Get the CustomAttributes of the final property.
        /// </summary>
        public object[] CustomAttributes { get; set; }
    }

    class PropertyPathVisitor<TModel, TProperty> : ExpressionVisitor
    {
        private Stack<PropertyInfo> Stack { get; set; }

        /// <summary>
        ///     Contains the property info until the final path.
        /// </summary>
        protected List<PropertyInfo> PathInfo { get; private set; }


        private PropertyPathVisitor()
        {
            // because the order is inverse.
            Stack = new Stack<PropertyInfo>();
        }

        public PropertyPathVisitor(Expression<Func<TModel, TProperty>> expr)
            : this()
        {
            if (expr == null)
                throw new InvalidOperationException("expr cannot be null");

            // dispatch for hook
            Visit(expr);
            FillPathInfo();
        }



        public PropertyPathVisitor(Expression<Func<TModel, object>> expr)
            : this()
        {
            if (expr == null)
                throw new InvalidOperationException("expr cannot be null");

            // dispatch for hook
            Visit(expr);
            FillPathInfo();
        }



        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.MemberType != MemberTypes.Property)
                throw new NotSupportedException();

            PropertyInfo pi = (PropertyInfo)node.Member;
            Stack.Push(pi);

            return base.VisitMember(node);
        }


        #region Helpers


        private void FillPathInfo()
        {
            PathInfo = new List<PropertyInfo>(Stack.Count);
            PropertyInfo elem;

            while (Stack.Count > 0 && (elem = Stack.Pop()) != null)
                PathInfo.Add(elem);
        }


        protected PropertyInfo SelectedProperty
        {
            get
            {
                if (PathInfo.Count == 0)
                    return null;

                return PathInfo[PathInfo.Count - 1];
            }
        }


        protected PropertyInfo BaseProperty
        {
            get
            {
                if (Stack.Count == 0)
                    return null;

                return PathInfo[0];
            }
        }

        #endregion
    }

    class PropertyValuePathVisitor<TModel, TProperty> : PropertyPathVisitor<TModel, TProperty>
    {
        // holds the instance of the object if passed
        object m_instanceObj;


        public PropertyValuePathVisitor(Expression<Func<TModel, TProperty>> expr)
            : base(expr)
        {

        }


        public PropertyValuePathVisitor(TModel instanceObj, Expression<Func<TModel, TProperty>> expr)
            : base(expr)
        {
            if (instanceObj == null)
                throw new ArgumentNullException("m_instanceObj");

            m_instanceObj = instanceObj;
        }


        /// <summary>
        ///     Return a formatted string separated by . between properties called.
        /// </summary>
        String m_propertyPath;
        public String PropertyPath
        {
            get
            {
                if (m_propertyPath == null)
                {

                    var builder = PathInfo.Aggregate(new StringBuilder(), (sb, pi) => sb.Append(pi.Name + "."));
                    m_propertyPath = builder.Remove(builder.Length - 1, 1).ToString();
                }

                return m_propertyPath;
            }
        }


        object m_value;
        bool m_canRecursively_get_thefinalValue = true;         // by default should be possible

        /// <summary>
        ///     Returns the value for the property selected.
        /// </summary>
        public object Value
        {
            get
            {
                if (m_instanceObj == null)
                {

                    //
                    // there is no instance object, so the value for the property selected is null.

                    return null;
                }

                if (m_canRecursively_get_thefinalValue == false)
                {

                    //
                    // we cannot access the final property because one of prior properties are null.

                    return null;
                }

                if (m_value == null)
                {
                    if (PathInfo.Count == 0)
                        return null;

                    if (PathInfo.Count == 1)
                        m_value = SelectedProperty.GetValue(m_instanceObj, null);

                    else
                    {
                        // recursively update m_instanceObj
                        object contextInstance = m_instanceObj;

                        for (int idx = 0; idx < PathInfo.Count; idx++)
                        {
                            PropertyInfo property = PathInfo[idx];

                            if (idx < (PathInfo.Count - 1))
                            {
                                //
                                // update contextInstance if not the last

                                contextInstance = property.GetValue(contextInstance, null);

                                if (contextInstance == null)
                                {
                                    m_canRecursively_get_thefinalValue = false;
                                    break;
                                }
                            }
                        }

                        if (m_canRecursively_get_thefinalValue)
                            m_value = SelectedProperty.GetValue(contextInstance, null);
                    }
                }

                return m_value;
            }
        }

        /// <summary>
        ///     Returns the type for the property selected.
        /// </summary>
        Type m_type;
        public Type Type
        {
            get
            {
                if (m_type == null)
                {
                    if (PathInfo.Count == 0)
                        return null;

                    m_type = SelectedProperty.PropertyType;
                }
                return m_type;
            }
        }

        /// <summary>
        ///     Gets the custom attributes of the selected property
        /// </summary>
        public object[] CustomAttributes
        {
            get
            {
                return SelectedProperty.GetCustomAttributes(true);
            }
        }


        /// <summary>
        ///     Get all information about the property
        /// </summary>
        public PropertyResolverResult Result
        {
            get
            {
                return new PropertyResolverResult
                {
                    FullPath = this.PropertyPath,
                    Value = this.Value,
                    Type = this.Type,
                    CustomAttributes = this.CustomAttributes
                };
            }
        }
    }

    #endregion




    /// <summary>
    ///     Gives information about methods and properties.
    /// </summary>
    public static class NamesResolver
    {
        /// <summary>
        ///    Return the name of the variable selected
        /// </summary>
        public static string Variable<TResult>(Expression<Func<TResult>> variableSelector)
        {
            MemberExpression mExpression = (MemberExpression)variableSelector.Body;
            return mExpression.Member.Name;
        }

        /// <summary>
        ///     Return information about the selected method 
        /// </summary>
        public static MethodInfo Method<TModel>(Expression<Func<TModel, object>> methodSelector)
        {
            return new MethodPathVisitor<TModel>(methodSelector).Result;
        }

        // <summary>
        ///     Return information about the selected property 
        /// </summary>
        public static PropertyResolverResult Property<TModel>(Expression<Func<TModel, object>> propertySelector)
        {
            return new PropertyValuePathVisitor<TModel, object>(propertySelector).Result;
        }

        // <summary>
        ///     Return information about the selected property 
        /// </summary>
        public static PropertyResolverResult Property<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            return new PropertyValuePathVisitor<TModel, TProperty>(propertySelector).Result;
        }

        // <summary>
        ///     Return information about the selected property for the current model 
        /// </summary>
        public static PropertyResolverResult Property<TModel>(TModel instance, Expression<Func<TModel, object>> propertySelector)
        {
            return new PropertyValuePathVisitor<TModel, object>(instance, propertySelector).Result;
        }

        // <summary>
        ///     Return information about the selected property for the current model 
        /// </summary>
        public static PropertyResolverResult Property<TModel, TProperty>(TModel instance, Expression<Func<TModel, TProperty>> propertySelector)
        {
            return new PropertyValuePathVisitor<TModel, TProperty>(instance, propertySelector).Result;
        }
    }
}
