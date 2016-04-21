using OMapper.Interfaces;
using OMapper.Internal;
using OMapper.Internal.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMapper.Providers
{
    public static class PredicateParserProvider
    {
        public static List<IPredicateParser> PredicateProviders = new List<IPredicateParser>();


        static PredicateParserProvider()
        {
            // add here more in the future through Ioc for example.
            PredicateProviders.Add(new ExpressionParserImpl());
        }


        public static IPredicateParser Current {  get { return PredicateProviders[0]; } }
    }
}
