using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Repository.OMapper.Extensions;
using op = System.Reflection.Emit.OpCodes;

namespace Repository.OMapper.Internal.Proxies
{
    internal static class ProxyCreator
    {
        public static readonly string PROXY_InternalID = "InternalID";
        public static readonly string OMapper_PROPERTY_NAME = "OMapperInstance";


        private static AssemblyBuilder s_assemblyBuilder;
        private static AssemblyName s_assemblyName;
        private static ModuleBuilder s_moduleBuilder;

        const string OMapperDynamicAssembly = "OMapperProxies";


        static ProxyCreator()
        {
            s_assemblyName = new AssemblyName(OMapperDynamicAssembly);
            s_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(s_assemblyName, AssemblyBuilderAccess.RunAndSave);
            s_moduleBuilder = s_assemblyBuilder.DefineDynamicModule(s_assemblyName.Name, s_assemblyName.Name + ".dll");
        }




        internal static bool IsProxy(object obj)
        {
            PropertyInfo pi = obj.GetType().GetProperty(PROXY_InternalID, OMapper.s_PropertiesFlags);
            return pi != null;
        }




        internal static Type EmitProxy(Type type)
        {
            TypeBuilder proxy = s_moduleBuilder.DefineType(type.Name + "Impl", TypeAttributes.Public, type);

            // define OMapper and IsProxy properties in each instance
            PropertyBuilder propProxyIdentity = proxy.AddProperty(PROXY_InternalID, typeof(long));
            PropertyBuilder propOMapperInstance = proxy.AddProperty(OMapper_PROPERTY_NAME, typeof(OMapper));


            MethodBuilder methodNotifyPropertyChangeBuilder = proxy.AddMethod("NotifyPropertyChange", MethodAttributes.Public, typeof(void), new[] { typeof(string) }, il =>
            {
                // private void NotifyPropertyChange(string propertyName) {
                //      this.OMapperProperty.PutObjectForUpdate(proxy, propertyName);
                // }
                il.Emit(op.Ldarg_0);    // push this.
                il.Emit(op.Call, propOMapperInstance.GetGetMethod());
                
                il.Emit(op.Ldarg_0);    // push this.
                il.Emit(op.Ldarg_1);    // push propertyName should be arg_1
                MethodInfo OMapperContextExecuterUpdateMethod = typeof(OMapperContextExecuter).GetMethod("PutObjectForUpdate", BindingFlags.Public | BindingFlags.Instance); /* new[] { typeof(object), typeof(string) });*/
                il.Emit(op.Call, OMapperContextExecuterUpdateMethod);
            });



            // override base properties
            foreach (var pi in type.GetProperties(OMapper.s_PropertiesFlags))
            {
                // save base setMethod - same logic
                MethodInfo SetMethodHook = pi.GetSetMethod();
                MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual;

                MethodBuilder overrideSetBuilder = proxy.AddMethod("set_" + pi.Name, attributes, SetMethodHook.ReturnType, new[] { pi.PropertyType }, il =>
                {
                    // maintain the old behavior
                    il.Emit(op.Ldarg_0);        // push this
                    il.Emit(op.Ldarg_1);        // push value
                    il.Emit(op.Call, SetMethodHook);

                    il.Emit(op.Ldarg_0);        // push this.
                    il.Emit(op.Ldstr, pi.Name); // push propertyName
                    il.Emit(op.Call, methodNotifyPropertyChangeBuilder);        // push value

                });
            }


            var t =  proxy.CreateType();

            s_assemblyBuilder.Save(s_assemblyName.Name + ".dll");

            return t;

            // 
        }
    }
}
