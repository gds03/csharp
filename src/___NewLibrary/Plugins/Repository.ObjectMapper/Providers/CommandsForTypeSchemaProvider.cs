using Repository.ObjectMapper.Interfaces;
using Repository.ObjectMapper.Internal;
using Repository.ObjectMapper.Internal.Commands.Impl;
using Repository.ObjectMapper.Internal.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.ObjectMapper.Providers
{
    public static class CommandsForTypeSchemaProvider
    {
        public static List<Func<ObjectMapper, ISqlCommandTextGenerator>> CommandProviders = new List<Func<ObjectMapper, ISqlCommandTextGenerator>>();


        static CommandsForTypeSchemaProvider()
        {
            // add here more in the future through Ioc for example.
            CommandProviders.Add(oMapper => new CommandsForTypeSchema(oMapper));
        }


        public static ISqlCommandTextGenerator GetCurrent(ObjectMapper oMapper)
        {
            if (oMapper == null)
                throw new ArgumentNullException("oMapper");


            return CommandProviders[0](oMapper);
        }
    }
}
