using Repository.OMapper.Types.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Repository.OMapper.Internal.Proxies
{
    internal static class ProxyMapper
    {


        private static volatile Dictionary<Type, Type> s_mapperToProxies = new Dictionary<Type, Type>(OMapper.s_TypesSchemaMapper.Count);
        private static volatile int s_initializing = 0;
        private static volatile int s_initialized = 0;


        internal static Type Map(Type type)
        {
            Debug.Assert(type != null);

            var result = UsermodeInitializator.ThreadSafe(ref s_mapperToProxies, ref s_initialized, ref s_initializing, () =>
            {
                Dictionary<Type, Type> r = new Dictionary<Type, Type>(s_mapperToProxies);
                r.Add(type, ProxyCreator.EmitProxy(type));
                return r;
            });

            return result[type];
        }





    }
}
