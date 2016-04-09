using Repository.OMapper.Interfaces;
using Repository.OMapper.Internal;
using Repository.OMapper.Internal.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.OMapper.Providers
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
