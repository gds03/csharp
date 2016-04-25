using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace VirtualNote.Common.ExtensionMethods
{
    public static class ExpressionExtensions
    {
        public static String PropertyName<T>(this Expression<Func<T, Object>> function)
        {
            return GetMemberInfo(function).Member.Name;
        }

        public static MemberExpression GetMemberInfo<TEntity>(Expression<Func<TEntity, Object>> method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }
    }
}
