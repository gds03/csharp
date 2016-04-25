using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VirtualNote.MVC.Helpers;

namespace VirtualNote.MVC.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<TEntity>(this IEnumerable<TEntity> collection,
            Func<TEntity, String> value, Func<TEntity, String> text) 
        {
            return ToSelectListItem(collection, value, text, null);
        }

        public static IEnumerable<SelectListItem> ToSelectListItem<TEntity>(this IEnumerable<TEntity> collection,
            Func<TEntity, String> value, Func<TEntity, String> text, Func<TEntity, bool> selected) 
        {
            return collection.Select(item => new SelectListItem {
                Value = value(item),
                Text = text(item),
                Selected = selected != null ? selected(item) : false
            });
        }



        public static IEnumerable<CheckboxListItem> ToCheckboxListItem<TEntity>(this IEnumerable<TEntity> collection,
            Func<TEntity, int> value, Func<TEntity, String> text, Func<TEntity, bool> enabled )
        {
            return collection.Select(item => new CheckboxListItem
            {
                ID = value(item),
                Enabled = enabled(item),
                Text = text(item)
            });
        }
    }
}