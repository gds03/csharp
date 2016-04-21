using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace OMapper.Internal.CLR2SQL
{
    internal static class CLRExpressionLinq2SQLExpressionLogicNames
    {
        /// <summary>
        ///     Map LINQ expressions into string SQL operations.
        /// </summary>
        internal static Dictionary<ExpressionType, string> GetConversions(int initialCapacity)
        {
            Debug.Assert(initialCapacity > 0);
            Dictionary<ExpressionType, string> conv = new Dictionary<ExpressionType, string>(initialCapacity);

            conv.Add(ExpressionType.AndAlso, "AND");
            conv.Add(ExpressionType.Equal, "=");
            conv.Add(ExpressionType.GreaterThan, ">");
            conv.Add(ExpressionType.GreaterThanOrEqual, ">=");
            conv.Add(ExpressionType.LessThan, "<");
            conv.Add(ExpressionType.LessThanOrEqual, "<=");
            conv.Add(ExpressionType.Modulo, "%");
            conv.Add(ExpressionType.Multiply, "*");
            conv.Add(ExpressionType.NotEqual, "<>");
            conv.Add(ExpressionType.OrElse, "OR");
            conv.Add(ExpressionType.Subtract, "-");
            conv.Add(ExpressionType.Add, "+");

            return conv;
        }
    }
}
