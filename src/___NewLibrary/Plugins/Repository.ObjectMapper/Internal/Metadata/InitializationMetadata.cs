using Repository.ObjectMapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.ObjectMapper.Internal.Metadata
{
    public class InitializationMetadata
    {
        public TypesMetadata<T> For<T>()
        {
            return new TypesMetadata<T>();
        }
    }
}
