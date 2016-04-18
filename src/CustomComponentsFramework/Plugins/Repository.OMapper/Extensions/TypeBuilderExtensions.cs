using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Repository.OMapper.Extensions
{
    public static class TypeBuilderExtensions
    {
        public static PropertyBuilder AddProperty(this TypeBuilder tb, string propertyName, Type propertyType)
        {
            if (tb == null)
                throw new ArgumentNullException("tb");

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName");

            if (propertyType == null)
                throw new ArgumentNullException("propertyType");

            // define field
            FieldBuilder fldBuilder = tb.DefineField("m_" + propertyName,
                                                        propertyType,
                                                        FieldAttributes.Private);

            // define property
            PropertyBuilder propBuilder = tb.DefineProperty(propertyName,
                                                        PropertyAttributes.HasDefault,
                                                        propertyType,
                                                        null);


            // The property set and property get methods require a special set of attributes.
            const MethodAttributes getSetAttr = MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName,
                                                       getSetAttr | MethodAttributes.Public,
                                                       propertyType,
                                                       Type.EmptyTypes);

            ILGenerator getPropMthdBldrIL = getPropMthdBldr.GetILGenerator();

            getPropMthdBldrIL.Emit(OpCodes.Ldarg_0);
            getPropMthdBldrIL.Emit(OpCodes.Ldfld, fldBuilder);
            getPropMthdBldrIL.Emit(OpCodes.Ret);





            // Define the "set" accessor method for CustomerName.
            MethodBuilder setPropMthdBldr = tb.DefineMethod("set_" + propertyName,
                                                            getSetAttr | MethodAttributes.Assembly,
                                                            null,
                                                            new Type[] { propertyType });

            ILGenerator setPropMthdBldrIL = setPropMthdBldr.GetILGenerator();

            setPropMthdBldrIL.Emit(OpCodes.Ldarg_0);
            setPropMthdBldrIL.Emit(OpCodes.Ldarg_1);
            setPropMthdBldrIL.Emit(OpCodes.Stfld, fldBuilder);
            setPropMthdBldrIL.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to 
            // their corresponding behaviors, "get" and "set" respectively. 
            propBuilder.SetGetMethod(getPropMthdBldr);
            propBuilder.SetSetMethod(setPropMthdBldr);

            return propBuilder;
        }



        public static MethodBuilder AddMethod(this TypeBuilder tb, string methodName, MethodAttributes attrs, Type methodReturnType, Type[] paramTypes, Action<ILGenerator> instructionsCallback)
        {
            if (tb == null)
                throw new ArgumentNullException("tb");

            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentException("methodName");

            if (methodReturnType == null)
                throw new ArgumentNullException("methodReturnType");

            if (instructionsCallback == null)
                throw new ArgumentNullException("instructionsCallback");

            MethodBuilder mthodBuilder = tb.DefineMethod(methodName, attrs, methodReturnType, paramTypes);

            ILGenerator mthodBuilderIL = mthodBuilder.GetILGenerator();
            instructionsCallback(mthodBuilderIL);

            mthodBuilderIL.Emit(OpCodes.Ret);
            return mthodBuilder;
        }
    }
}
