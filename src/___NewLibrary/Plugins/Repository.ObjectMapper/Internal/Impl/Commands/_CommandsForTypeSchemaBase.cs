using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.ObjectMapper.Internal.Commands.Impl
{
    internal class CommandsForTypeSchemaBase
    {
        protected readonly ObjectMapper m_oMapper;

        public CommandsForTypeSchemaBase(ObjectMapper oMapper)
        {
            if (oMapper == null)
                throw new ArgumentNullException("oMapper");

            m_oMapper = oMapper;
        }
    }
}
