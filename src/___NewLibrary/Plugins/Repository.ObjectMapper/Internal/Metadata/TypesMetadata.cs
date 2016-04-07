using Repository.ObjectMapper.Types;
using Repository.ObjectMapper.Types.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository.ObjectMapper.Internal.Metadata
{
    public class TypesMetadata<T>
    {
        // FluentAPI
        public TypesMetadata<T> PrimaryKey(Expression<Func<T, object>> selector)
        {
            TypeSchema schema = this.GetSchema();
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
            TypeSchema schema = this.GetSchema();
            string selected = GetPropertySelected(selector);

            schema.IdentityPropertyName = selected;
            return this;
        }




        private TypeSchema GetSchema()
        {
            return ObjectMapper.AddMetadataFor(typeof(T));
        }


        private string GetPropertySelected(Expression<Func<T, object>> selector)
        {
            UnaryExpression uex = selector.Body as UnaryExpression;

            if (uex == null) throw new NotSupportedException("Only Unary expressions are supported by ObjectMapper");
            MemberExpression mex = uex.Operand as MemberExpression;
            if (uex == null) throw new NotSupportedException("Only Member Expressions are supported by ObjectMapper");
            return mex.Member.Name;
        }
    }
}
