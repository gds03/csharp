//using CustomComponents.Core.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.Services.Mappers.OrderBy
//{
//    public class ComplexKeyOrderManager
//    {
//        const string REFS_ENUM_IDENTIFIER = "refs_";
//        readonly static Dictionary<Type, Func<IComplexKeyOrderBy>> s_complexQueriesForTypes = new Dictionary<Type, Func<IComplexKeyOrderBy>>();


//        // Map each service into him IComplexKeyOrderBy
//        static ComplexKeyOrderManager()
//        {
//            //
//            // MAP: SERVICE <-> SERVICE-IComplexKeyOrderBy

//            //s_mapper.Add(typeof(BeneficiaryService), () => BenificiaryLambdaMapper.SINGLETON);

//        }









//        /// <summary>
//        ///     Only if the selected value of enum start with "refs_" try to dispatch the request.
//        ///     Otherwise return NULL.
//        /// </summary>
//        /// <typeparam name="TSource">The type of your table</typeparam>
//        /// <typeparam name="TEnum">Your order enum</typeparam>
//        /// <param name="typeOfService">The type of your service to obtain the associated mapper</param>
//        /// <param name="enumer">Your order enum</param>
//        protected IComplexKeyOrderBy<TSource> GetIComplexKeyOrderBy<TSource, TEnum>(Type typeOfService, TEnum? enumer)
//            where TEnum : struct
//        {
//            if (enumer == null)
//                return null;

//            if (enumer.GetType().IsEnum == false)
//                throw new InvalidOperationException("enumer");

//            string enumText = Enum.GetName(typeof(TEnum), enumer);
//            if (!enumText.StartsWith(REFS_ENUM_IDENTIFIER))
//                return null;

//            // Cast here.
//            return (IComplexKeyOrderBy<TSource>)GetIComplexKeyOrderBy(typeOfService);
//        }


//        #region Helpers


//        /// <summary>
//        ///     Get the real concretization from the mapper and validate if derives from BaseService.
//        /// </summary>
//        private static IComplexKeyOrderBy GetIComplexKeyOrderBy(Type typeOfService)
//        {
//            if (typeOfService == null)
//                throw new ArgumentNullException("typeOfService");

//            if (typeOfService.BaseType != typeof(BaseService))
//                throw new InvalidOperationException("Base type must be the type BaseService");

//            Func<IComplexKeyOrderBy> result = null;

//            if (!s_complexQueriesForTypes.TryGetValue(typeOfService, out result))
//                return null;

//            return result();
//        }


//        #endregion
//    }
//}
