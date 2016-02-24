using CustomComponents.Repository.Interfaces;
using CustomComponents.Services.Enums;
using CustomComponents.Services.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomComponents.Services.Services
{
    public class BaseService  // : ComplexKeyOrderManager      // inherit complex order by mappers to provide to leaf classes
    {
        protected Func<IRepository> RequestRepository { get; set; }


        public BaseService(Func<IRepository> Factory)
        {
            if (Factory == null)
                throw new ArgumentNullException("Factory");

            RequestRepository = Factory;
        }


        /// <summary>
        ///     Provides a repository to make operations and dispose automatically.
        /// </summary>
        protected TResult UseAndDispose<TResult>(Func<IRepository, TResult> func)
        {
            using (IRepository r = RequestRepository())
            {
                return func(r);
            }
        }





        protected static QuerySearchOptions TranslateQuerySearch<TEnumOrder>(TEnumOrder? orderByOption, SortByOption? sortDirection)
            where TEnumOrder : struct
        {
            bool ascending = OrderByAscending(sortDirection);
            string orderByColumn = GetNameForEnum<TEnumOrder>(orderByOption);

            return new QuerySearchOptions(orderByColumn, ascending);
        }


        //CustomIdentity _customIdentity;
        //protected CustomIdentity AuthenticatedUser
        //{
        //    get
        //    {
        //        if ( _customIdentity == null )
        //        {
        //            if ( !Thread.CurrentPrincipal.Identity.IsAuthenticated )
        //                throw new InvalidOperationException("Authenticate the user first");

        //            _customIdentity = (CustomIdentity) Thread.CurrentPrincipal.Identity;
        //        }

        //        return _customIdentity;
        //    }
        //}




        #region Helpers

        /// <summary>
        ///     Transform sortDirection to bool that indicates that is to order by ascending way
        /// </summary>
        private static bool OrderByAscending(SortByOption? sortDirection)
        {
            if (sortDirection == null)
                return true;

            return sortDirection.Value == SortByOption.ASCENDING;
        }


        /// <summary>
        ///     Returns the name wrotten in the value of the enum or NULL if enum is null (nothing to order by)
        ///     REMARK: this method replaces all _ to .
        /// </summary>
        private static string GetNameForEnum<TEnum>(TEnum? enumer)
            where TEnum : struct
        {
            if (enumer == null)
                return null;

            if (!enumer.GetType().IsEnum)
                throw new InvalidOperationException("enumer is not a enum");

            //
            // We must check if FK_ or Refs_ is there and ignore it!   

            // e.g :)           FK_Especialidade1_nome                                                      -> FK_Especialidade.nome
            // e.g :)           FK_Especialidade1_FK_LocalEspecialidade_nome                                -> FK_Especialidade.FK_LocalEspecialidade.nome
            // e.g :'(((        FK_Especialidade1_FK_LocalEspecialidade_Refs_PingoDoce.Select(x => x.Nome)  -> FK_Especialidade.FK_LocalEspecialidade.Refs_PingoDoce.Select(x => x.Nome)

            //var sb = new StringBuilder(fullPath);
            //return sb.Replace("FK_", "FK#").Replace("Refs_", "Refs#").Replace('_', '.')
            //         .Replace("FK#", "FK_").Replace("Refs#", "Refs_")
            //         .ToString();

            string fullPath = Enum.GetName(typeof(TEnum), enumer);

            const string IgnorePattern = @"(?<!FK|refs)_";
            const string Replacement = ".";

            return Regex.Replace(fullPath, IgnorePattern, Replacement);
        }

        #endregion
    }
}
