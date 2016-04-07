using CustomComponents.Core.Types.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace CustomComponents.Mvc.Extensions
{
    public static class HtmlHelperOfTExtensions
    {
        public static MvcHtmlString IdForJQuery<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            return new MvcHtmlString("'#" + html.IdFor<TModel, TProperty>(expression).ToString() + "'");
        }


        // <a href... > </a>
        public static MvcHtmlString Anchor(this HtmlHelper helper, string innerText = null, object htmlAttributes = null)
        {
            return ControlHelper(helper, "a", innerText, htmlAttributes);
        }



        public static MvcHtmlString MyTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,

            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
            where TModel : class
        {

            var propertyMetadata = NamesResolver.Property(htmlHelper.ViewData.Model, expression);

            var filter = propertyMetadata.CustomAttributes.OfType<DisplayFormatAttribute>().ToArray();

            if (filter.Length == 0)
            {
                //
                // format is null

                return htmlHelper.TextBoxFor(expression, null, htmlAttributes);
            }

            if (filter.Length > 1)
                throw new NotSupportedException("This should not happen");

            //
            // format is passed from attribute to native format overload.
            DisplayFormatAttribute format = (DisplayFormatAttribute)filter[0];
            return htmlHelper.TextBoxFor(expression, format.DataFormatString, htmlAttributes);
        }

        
        #region Helpers


        private static MvcHtmlString ControlHelper(this HtmlHelper helper, string controlTag,
            string innerText, object htmlAttributes)
        {
            if (String.IsNullOrEmpty(controlTag))
                throw new ArgumentException("ControlTag");

            TagBuilder a = new TagBuilder(controlTag);
            RouteValueDictionary rvd = null;

            if (!string.IsNullOrEmpty(innerText))
                a.SetInnerText(innerText);

            if (htmlAttributes != null)
                rvd = new RouteValueDictionary(htmlAttributes);

            if (rvd != null)
            {
                foreach (var kvp in rvd)
                {
                    a.MergeAttribute(kvp.Key, kvp.Value.ToString());
                }
            }

            return new MvcHtmlString(a.ToString());
        }

        #endregion
    }
}
