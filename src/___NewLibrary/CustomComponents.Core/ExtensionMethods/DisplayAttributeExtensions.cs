using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Core.ExtensionMethods
{
    public static class DisplayAttributeExtensions
    {
        public static ResourceManager GetResourceManagerInstance(this DisplayAttribute display)
        {
            if (display.ResourceType == null)
                return null;

            const BindingFlags flags = BindingFlags.Static |
                                       BindingFlags.GetProperty |
                                       BindingFlags.InvokeMethod |
                                       BindingFlags.Public |
                                       BindingFlags.FlattenHierarchy;

            var t = display.ResourceType;
            object result = t.GetProperty("ResourceManager", flags).GetValue(null, null);       // each resource have a public static property called ResourceManager
            return (ResourceManager)result;
        }
    }
}
