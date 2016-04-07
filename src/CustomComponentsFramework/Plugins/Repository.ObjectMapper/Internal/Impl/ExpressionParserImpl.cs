using Repository.ObjectMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Repository.ObjectMapper.Internal.Impl
{
    internal class ExpressionParserImpl : IPredicateParser
    {

        /// <summary>
        ///     Translates the expression into a string to be passed into SQL
        /// </summary>
        public String ParseFilter(Expression expr)
        {
            //
            // This is a simple filter, we only want (and parse) simple expressions.
            // 

            BinaryExpression bExpr;
            MemberExpression mExpr;
            ConstantExpression cExpr;

            if ((bExpr = expr as BinaryExpression) != null)
            {
                //
                // We have 2 expressions (left and right)
                // We get the first left result use the operator and get the right part 
                // (that can contain another BinaryExpression For example)
                // 

                StringBuilder innerText = new StringBuilder();
                String recursiveResult;

                recursiveResult = ParseFilter(bExpr.Left);                 // Go left

                innerText.Append("( ");
                innerText.Append(recursiveResult);
                innerText.Append(" ");

                innerText.Append(ObjectMapper.s_ExpressionToSQLMapper[bExpr.NodeType]);      // Map node types to SQL operators
                innerText.Append(" ");

                recursiveResult = ParseFilter(bExpr.Right);               // Go right

                innerText.Append(recursiveResult);
                innerText.Append(" )");

                return innerText.ToString();
            }

            if ((cExpr = expr as ConstantExpression) != null)
            {
                return ObjectMapper.PrepareValue(cExpr.Value);
            }

            if ((mExpr = expr as MemberExpression) == null)
                throw new NotSupportedException("unsupported filter");

            // expr is of MemberExpression type            
            Expression innerExpr = mExpr.Expression;

            ParameterExpression pInnerExpr;
            MemberExpression mInnerExpr;
            ConstantExpression cInnerExpr;
            BinaryExpression bInnerExpr;

            if ((bInnerExpr = innerExpr as BinaryExpression) != null)
                return ParseFilter(bInnerExpr);                                          // Go recursive    

            if ((pInnerExpr = innerExpr as ParameterExpression) != null)
            {
                return ObjectMapper.GetMappingForProperty(pInnerExpr.Type, mExpr.Member.Name);        // We must map property of the type to SQL Column
            }

            if ((cInnerExpr = innerExpr as ConstantExpression) != null)
            {
                object obj = cInnerExpr.Value;                                           // Get anonymous object (captured by the compiler)

                FieldInfo value = obj.GetType().GetField(mExpr.Member.Name);             // Get the field of the anonymous object 
                return ObjectMapper.PrepareValue(value.GetValue(obj));
            }

            if ((mInnerExpr = innerExpr as MemberExpression) == null)
                throw new NotSupportedException("unsupported filter");

            // innerExpr is MemberExpression!
            if ((cInnerExpr = mInnerExpr.Expression as ConstantExpression) == null)
                throw new NotSupportedException("unsupported filter");

            object objValue = cInnerExpr.Value;                                                     // Get anonymous object (captured by the compiler)

            FieldInfo fieldValue = objValue.GetType().GetField(mInnerExpr.Member.Name);             // Get the field of the anonymous object 
            object obj2 = fieldValue.GetValue(objValue);                                            // Get the object in the field

            return ObjectMapper.PrepareValue(obj2.GetType().GetProperty(mExpr.Member.Name).GetValue(obj2, null));       // Based on the object, finally get the value in the property 
        }

    }
}
