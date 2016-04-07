//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace CustomComponents.Core.Types
//{
//    public static class Types
//    {


//        #region Properties




//        class PropertyPathVisitor<T> : ExpressionVisitor
//        {
//            public string PropertyName { get; private set; }



//            public PropertyPathVisitor(Expression<Func<T, object>> expr)
//            {
//                Visit(expr);
//            }

//            protected override Expression VisitMember(MemberExpression node)
//            {
//                if (PropertyName != null)
//                    throw new InvalidOperationException();

//                // Only one property in the expression
//                PropertyName = node.Member.Name;

//                return base.VisitMember(node);
//            }
//        }



//        /// <summary>
//        ///     Get the property name for type
//        /// </summary>
//        public static String GetPropertyName<T>(Expression<Func<T, object>> expr)
//        {
//            if (expr == null)
//                throw new ArgumentNullException("expr");

//            return new PropertyPathVisitor<T>(expr).PropertyName;
//        }



//        /// <summary>
//        ///     Compare to instances of the same type and return the name of the properties that were changed
//        /// </summary>
//        /// <returns>The name of the properties where they value is different</returns>
//        public static IEnumerable<String> GetChangedProperties<TEntity1, TEntity2>(TEntity1 original, TEntity2 changed)
//        {
//            if (typeof(TEntity1) != typeof(TEntity2))
//                throw new InvalidOperationException("Types must be identical");

//            //
//            // If entities are of the same type, they have the same number of properties
//            //

//            Type repOriginal = original.GetType();
//            Type repChanged = changed.GetType();

//            if (repOriginal.IsValueType)
//                throw new InvalidOperationException("Value types are not allowed to be compared");

//            var propertiesInfo = repOriginal.GetProperties(BindingFlags.Instance | BindingFlags.Public);

//            // Iterate over propertiesInfo and change the projection
//            return propertiesInfo.Where(pi => pi.GetValue(original, null).Equals(repChanged.GetProperty(pi.Name).GetValue(changed, null)) == false)
//                                 .Select(pi => pi.Name)
//                                 .ToList();

//        }




//        #endregion







//        #region Methods



//        //public sealed class MethodInternalInfoInfo
//        //{
//        //    public int ArgumentIndex { get; internal set; }
//        //    public int ArgumentType { get; internal set; }
//        //}

//        //public sealed class MethodInternalInfo
//        //{
//        //    public string MethodName { get; internal set; }
//        //    public int ArgumentCount { get; internal set; }
//        //    public List<MethodInternalInfoInfo> Arguments { get; internal set; }
//        //}




//        //class MethodPathVisitor<T> : ExpressionVisitor
//        //{
//        //    public MethodInternalInfo MethodInfo { get; private set; }



//        //    public MethodPathVisitor(Expression<Func<T, object>> expr)
//        //    {
//        //        Visit(expr);
//        //    }

//        //    protected override Expression VisitMethodCall(MethodCallExpression node)
//        //    {
//        //        if ( MethodInfo != null )
//        //            throw new InvalidOperationException();

//        //        int idx = 0;
//        //        MethodInfo = new MethodInternalInfo
//        //        {
//        //            MethodName = node.Method.Name,
//        //            ArgumentCount = node.Arguments.Count,
//        //            Arguments = null
//        //        };

//        //        return base.VisitMethodCall(node);
//        //    }
//        //}


//        ///// <summary>
//        /////     Get the method name for the type and him parameters count
//        ///// </summary>
//        //public static MethodInternalInfo GetMethodInformation<T>(Expression<Func<T, object>> expr)
//        //{
//        //    if ( expr == null )
//        //        throw new ArgumentNullException("expr");

//        //    return new MethodPathVisitor<T>(expr).MethodInfo;
//        //}

//        ///// <summary>
//        /////     Get the method name for the type and him parameters count
//        ///// </summary>
//        //public static string GetMethodName<T>(Expression<Func<T, Delegate>> expr)
//        //{
//        //    return null;
//        //}


//        #endregion
//    }
//}