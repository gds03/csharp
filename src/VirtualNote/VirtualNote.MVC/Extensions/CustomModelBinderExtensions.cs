using System;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Reflection;
using VirtualNote.Common;
using VirtualNote.Common.ExtensionMethods;

namespace VirtualNote.MVC.Extensions
{
    public class CostumModelBinderException : Exception
    {

    }

    public static class CustomModelBinderExtensions
    {
        public static bool TryUpdateModel<TEntity>(this FormCollection collection,
            TEntity entity, 
            params Expression<Func<TEntity, object>>[] properties)
        {
            Type t = typeof(TEntity);
            foreach (Expression<Func<TEntity, object>> propertyExpr in properties)
            {
                try
                {
                    MemberExpression me = propertyExpr.GetMemberInfo();
                    MemberInfo mi = me.Member;

                    String propertyName = mi.Name;
                    String collectionValue = collection[propertyName];
                    object value = null;

                    if (me.Type == typeof(bool))
                    {
                        value = collectionValue.ToBool();
                    }
                    else
                        if (me.Type == typeof(int))
                        {
                            value = int.Parse(collectionValue);
                        }
                        else
                        {
                            value = collectionValue;
                        }

                    t.GetProperty(propertyName).SetValue(entity, value, null);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}