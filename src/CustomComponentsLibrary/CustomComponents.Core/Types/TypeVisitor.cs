using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.Types
{
    public abstract class TypeVisitor
    {
        public object Value { get; protected set; }

        public void Resolve(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Type t = value.GetType();

            if (t == typeof(Boolean))
            {
                Boolean((Boolean)value);
                goto end;
            }


            if (t == typeof(Byte))
            {
                Byte((Byte)value);
                goto end;
            }


            if (t == typeof(Int16))
            {
                Int16((Int16)value);
                goto end;
            }


            if (t == typeof(Int32))
            {
                Int32((Int32)value);
                goto end;
            }

            // ------------------------------------------------

            if (t == typeof(Int64))
            {
                Int64((Int64)value);
                goto end;
            }


            if (t == typeof(Decimal))
            {
                Decimal((Decimal)value);
                goto end;
            }


            if (t == typeof(Single))
            {
                Single((Single)value);
                goto end;
            }


            if (t == typeof(Double))
            {
                Double((Double)value);
                goto end;
            }

            // ------------------------------------------------

            if (t == typeof(Enum))
            {
                Enum((Enum)value);
                goto end;
            }


            if (t == typeof(Char))
            {
                Char((Char)value);
                goto end;
            }


            if (t == typeof(String))
            {
                String((String)value);
                goto end;
            }


            if (t == typeof(Char[]))
            {
                CharArray((Char[])value);
                goto end;
            }

            // ------------------------------------------------

            if (t == typeof(DateTime))
            {
                DateTime((DateTime)value);
                goto end;
            }


            if (t == typeof(DateTimeOffset))
            {
                DateTimeOffset((DateTimeOffset)value);
                goto end;
            }


            if (t == typeof(TimeSpan))
            {
                TimeSpan((TimeSpan)value);
                goto end;
            }


            if (t == typeof(Byte[]))
            {
                ByteArray((Byte[])value);
                goto end;
            }

            // ------------------------------------------------

            if (t == typeof(Guid))
            {
                Guid((Guid)value);
                goto end;
            }

            // ------------------------------------------------


            //
        // Set the original value if not adjusted by hook methods.
        end:

            if (Value == null)
                Value = value;
        }

        public virtual void Boolean(Boolean value) { }
        public virtual void Byte(Byte value) { }
        public virtual void Int16(Int16 value) { }
        public virtual void Int32(Int32 value) { }

        public virtual void Int64(Int64 value) { }
        public virtual void Decimal(Decimal value) { }
        public virtual void Single(Single value) { }
        public virtual void Double(Double value) { }

        public virtual void Enum(Enum value) { }
        public virtual void Char(Char value) { }
        public virtual void String(String value) { }
        public virtual void CharArray(Char[] value) { }

        public virtual void DateTime(DateTime value) { }
        public virtual void DateTimeOffset(DateTimeOffset value) { }
        public virtual void TimeSpan(TimeSpan value) { }
        public virtual void ByteArray(Byte[] value) { }

        public virtual void Guid(Guid value) { }
    }
}
