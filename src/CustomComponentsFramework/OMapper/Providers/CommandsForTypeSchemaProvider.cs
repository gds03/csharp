﻿using OMapper.Interfaces;
using OMapper.Internal;
using OMapper.Internal.Commands.Impl;
using OMapper.Internal.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMapper.Providers
{
    public static class CommandsForTypeSchemaProvider
    {
        public static List<Func<ISqlCommandTextGenerator>> CommandProviders = new List<Func<ISqlCommandTextGenerator>>();


        static CommandsForTypeSchemaProvider()
        {
            // add here more in the future through Ioc for example.
            CommandProviders.Add(() => new CommandsForTypeSchema());
        }


        public static ISqlCommandTextGenerator Current
        {
            get
            {
                return CommandProviders[0]();
            }
        }
    }
}
