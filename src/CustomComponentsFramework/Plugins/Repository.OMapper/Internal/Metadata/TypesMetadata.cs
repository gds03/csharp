using Repository.OMapper.Types;
using Repository.OMapper.Types.Mappings;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Repository.OMapper.Internal.Metadata
{
    public class TypesMetadata<T>
    {
        // FluentAPI
        public TypesMetadata<T> PrimaryKey(Expression<Func<T, object>> selector)
        {
            TypeSchema schema = GetSchema();
            string selected = GetPropertySelected(selector);

            if (!schema.Keys.Any(x => x.Key == selected))
            {
                var c = schema.Columns[selected];
                schema.Keys.Add(selected, new KeyMapping(c.ToSqlTableColumn, c.ClrProperty));
            }

            return this;
        }

        public TypesMetadata<T> Identity(Expression<Func<T, object>> selector)
        {
            TypeSchema schema = GetSchema();
            string selected = GetPropertySelected(selector);

            schema.IdentityPropertyName = selected;
            return this;
        }


        public TypesMetadata<T> BindFrom(string FromSQLColumnName)
        {
            throw new NotImplementedException();
        }

        public TypesMetadata<T> BindTo(string ToSQLColumnName)
        {
            throw new NotImplementedException();
        }

        public TypesMetadata<T> Exclude(Expression<Func<T, object>> selector)
        {
            throw new NotImplementedException();
        }



        #region Helper Methods


        private static TypeSchema GetSchema()
        {
            return OMapper.AddMetadataFor(typeof(T));
        }


        private static string GetPropertySelected(Expression<Func<T, object>> selector)
        {
            UnaryExpression uex = selector.Body as UnaryExpression;

            if (uex == null) throw new NotSupportedException("Only Unary expressions are supported by OMapper");
            MemberExpression mex = uex.Operand as MemberExpression;
            if (uex == null) throw new NotSupportedException("Only Member Expressions are supported by OMapper");
            return mex.Member.Name;
        }


        #endregion
    }
}
