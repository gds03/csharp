//using CustomComponents.Core.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.Services.Mappers.OrderBy
//{
//    // receives a source, and bool (orderbyAscending) and return the source ordered.
//    public abstract class ComplexKeyOrderByBase<TEnum, TSource> : Dictionary<TEnum, Func<IQueryable<TSource>, bool, IQueryable<TSource>>>,
//        IComplexKeyOrderBy<TSource>, IComplexKeyOrderBy where TEnum : struct
//    {
//        //
//        // available method for all leaf classes.

//        public IQueryable<TSource> ApplyOrder(IQueryable<TSource> queryable, string columnName, bool orderByAscending)
//        {

//            // 
//            // Dispatch to function that knows how to order.

//            TEnum key = Convert<TEnum>(columnName);
//            return this[key](queryable, orderByAscending);     // apply order over the source.
//        }


//        private static TEnum Convert<TEnum>(string value)
//        {
//            if (string.IsNullOrEmpty(value))
//                throw new ArgumentNullException("value");

//            return (TEnum)Enum.Parse(typeof(TEnum), value);
//        }
//    }
//}
