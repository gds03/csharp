using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomComponents.Mvc.Extensions
{

    public static class IEnumerableExtensions
    {
        public static List<SelectListItem> ToSelectListItem<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            return collection.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value.ToString() }).ToList();
        }



        public static List<SelectListItem> ToSelectListItem<TValue>(this IEnumerable<KeyValuePair<int, TValue>> collection, int selectedKeyValue)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            return collection.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value.ToString(), Selected = (x.Key == selectedKeyValue) })
                             .ToList();
        }
    }
}
