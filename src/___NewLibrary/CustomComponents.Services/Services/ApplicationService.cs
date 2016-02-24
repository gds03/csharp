using CustomComponents.Core.Types.Collections;
using CustomComponents.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomComponents.Services.Services
{

    public class ApplicationService : BaseService
    {
        public const int CACHE_REFRESH_MILIS = 0;       // Never refresh, once in memory, the dictionary is not freed.
        static readonly TimerConcurrentDictionary<int, object> s_sharedData;

        static ApplicationService()
        {
            // THREADS  ----------------------------------------------------------------
            int workerThreads, completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

            // METHODS  ----------------------------------------------------------------
            BindingFlags bf = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
            Type t;

            int numMembers = typeof(ApplicationService).GetMethods(bf).Count(mi => mi.GetParameters() == null || mi.GetParameters().Length == 0);
            s_sharedData = new TimerConcurrentDictionary<int, object>(workerThreads, numMembers, CACHE_REFRESH_MILIS);
        }


        public ApplicationService(Func<IRepository> RepositoryFactory) : base(RepositoryFactory) { }


        /// <summary>
        ///     Helper method that translates method and properties to uid (int)
        /// </summary>
        private static int GetIdentifierForMethod(Expression<Func<ApplicationService, object>> expression)
        {
            String memberName = new NamesResolverHelper<ApplicationService>(expression).Name;
            return memberName.GetHashCode();
        }

        /// <summary>
        ///     Try get the data from the expression (associated to the method or property).
        ///     If not possible to get, call resultData function and set data in expression identifier key.
        /// </summary>
        private object TryGetFromCache(Expression<Func<ApplicationService, object>> expression, Func<object> resultData, bool useCache = true)
        {
            if (!useCache)
                return resultData();

            // 
            // Template for cache usage 

            object result;
            int methodIdentifier = GetIdentifierForMethod(expression);

            if (!s_sharedData.TryGetValue(methodIdentifier, out result))
            {
                result = resultData();

                s_sharedData.TryAdd(methodIdentifier, result);
            }

            return result;
        }



        //public IEnumerable<KeyValuePair<int, string>> GetLearnMoreList(bool useCache)
        //{
        //    return (IEnumerable<KeyValuePair<int, string>>)
        //           TryGetFromCache(a => a.GetLearnMoreList(useCache), () =>
        //           {
        //               using ( var r = RequestRepository() )
        //               {
        //                   return r.Query<GenericMasterData>().GetLearnMoreList()
        //                           .Select(x => new KeyValuePair<int, string>(x.Id, x.Value))
        //                           .ToList();
        //               }
        //           }, useCache);
        //}

    }







    #region Helpers

    public class NamesResolverHelper<T> : ExpressionVisitor
    {
        public String Name { get; private set; }


        public NamesResolverHelper(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Visit(expression);

            if (Name == null)
                throw new NotSupportedException("This class only resolves properties and methods.");
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.MemberType != System.Reflection.MemberTypes.Property)
                throw new NotSupportedException();

            Name = node.Member.Name;
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var t = node.Method.GetParameters().Aggregate(new StringBuilder(), (sb, x) => sb.Append(x.ParameterType.ToString() + ", "));
            Name = (node.Method.Name + "_" + t.Remove(t.Length - 2, 2).ToString());
            return base.VisitMethodCall(node);
        }
    }



    #endregion
}
