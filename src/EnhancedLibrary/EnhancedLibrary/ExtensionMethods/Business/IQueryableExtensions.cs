using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EnhancedLibrary.ExtensionMethods.Business
{
    /// <summary>
    ///     Extends the behavior of the IQueryable Interface
    /// </summary>
    public static class IQueryableExtensions
    {

        /// <summary>
        ///     Include related entities, based on the lambda expression and materialize to objects.
        /// </summary>
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Expression<Func<T, object>> selector) 
        {
            ObjectQuery<T> q = query as ObjectQuery<T>;

            if ( q == null )
                throw new InvalidOperationException("IQueryable Source must be of ObjectQuery type");

            return InternalInclude(q, selector);
        }



        /// <summary>
        ///     Include related entities, based on the lambda expression and materialize to objects.
        /// </summary>
        public static IQueryable<T> Include<T>(this ObjectQuery<T> query, Expression<Func<T, object>> selector)
        {
            if ( query == null )
                throw new ArgumentNullException("ObjectQuery cannot be null");

            return InternalInclude(query, selector);
        }

        





        #region Internal Methods
        



        static ObjectQuery<T> InternalInclude<T>(this ObjectQuery<T> query, Expression<Func<T, object>> selector) 
        {
            // Obtain path to pass to Include(string) method
            string path = new PropertyPathVisitor().GetPropertyPath(selector);

            // Call Include(string method)
            return query.Include(path);
        }






        class PropertyPathVisitor : ExpressionVisitor
        {
            //
            // This stack will contain the property fullName, and, because the visit method
            // will visit from inside, to outside (inverse order) we use a stack.

            Stack<string> m_stack_propertyfullName; 


            public string GetPropertyPath(Expression expression) 
            {
                // Initialize a new Stack
                m_stack_propertyfullName = new Stack<string>();

                //
                // Visit current expression.
                // After this method call, the stack contains the path for we pass to include, and for each one,
                // we need to build the path

                Visit(expression);

                // Iterate in the stack and build the property fullPath 
                return m_stack_propertyfullName.Aggregate(new StringBuilder(),
                    (sb, value) => ( sb.Length > 0 ? sb.Append(".") : sb ).Append(value)
                )
                .ToString();
            }




            #region Overrided Methods that are called when the node is of these Types

            // Members
            protected override Expression VisitMember(MemberExpression expression) 
            {
                if ( m_stack_propertyfullName != null )
                    m_stack_propertyfullName.Push(expression.Member.Name);

                return base.VisitMember(expression);
            }

            // Methods
            protected override Expression VisitMethodCall(MethodCallExpression expression) 
            {
                if ( IsLinqOperator(expression.Method) ) {
                    for ( int i = 1; i < expression.Arguments.Count; i++ ) {
                        Visit(expression.Arguments[i]);
                    }

                    Visit(expression.Arguments[0]);
                    return expression;
                }

                return base.VisitMethodCall(expression);
            }


            #endregion






            static bool IsLinqOperator(MethodInfo method) 
            {
                if ( method.DeclaringType != typeof(Queryable) && method.DeclaringType != typeof(Enumerable) )
                    return false;

                //
                // Must be enumerable, queryable and a ExtensionMethod

                return Attribute.GetCustomAttribute(method, typeof(ExtensionAttribute)) != null;
            }
        }

        #endregion
    }
}
