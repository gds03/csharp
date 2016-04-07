using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CustomComponents.Core.ExtensionMethods;
using System.Web.Mvc.Html;
using CustomComponents.Mvc.UserControls.Types.UserControls;

namespace CustomComponents.Mvc.UserControls.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        ///     Returns a PartialView with the specific name passing the specified model
        /// </summary>
        /// <param name="modelSelector">The selector for the model that will be passed to the partial view.</param>
        /// <param name="partialViewName">The name of the partial View.</param>
        /// 
        public static MvcHtmlString Partial<TModel, TProperty>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> modelSelector, string partialViewName, object htmlAttributes = null)
        {
            if (modelSelector == null)
                throw new ArgumentNullException("selector");

            if (string.IsNullOrEmpty(partialViewName))
                throw new ArgumentNullException("partialViewName");

            var partialVD = GetPartialViewData(modelSelector, html.ViewData);

            if (htmlAttributes != null)
            {
                partialVD["htmlAttributes"] = htmlAttributes.ToDictionary();
            }

            return html.Partial(partialViewName, partialVD);
        }

        #region Helpers



        private static TemplateInfo BuildTemplateInfo<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertySelector, TemplateInfo currentTemplateInfo)
        {
            if (currentTemplateInfo == null)
                throw new ArgumentNullException("currentTemplateInfo");

            const string separator = ".";
            TemplateInfo ti = new TemplateInfo() { HtmlFieldPrefix = currentTemplateInfo.HtmlFieldPrefix };

            if (!string.IsNullOrEmpty(currentTemplateInfo.HtmlFieldPrefix) && !currentTemplateInfo.HtmlFieldPrefix.EndsWith(separator))
                ti.HtmlFieldPrefix += ".";

            ti.HtmlFieldPrefix += ExpressionHelper.GetExpressionText(propertySelector);
            return ti;
        }


        private static ViewDataDictionary GetPartialViewData<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertySelector, ViewDataDictionary<TModel> viewData)
        {
            var partialVD = new ViewDataDictionary(viewData);

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(propertySelector, viewData);
            partialVD.Model = metadata.Model;
            partialVD.ModelMetadata = metadata;
            partialVD.TemplateInfo = BuildTemplateInfo(propertySelector, viewData.TemplateInfo);

            return partialVD;
        }






        #endregion



        public static UserControls<TModel> UserControls<TModel>(this HtmlHelper<TModel> html)
        {
            return new UserControls<TModel>(html);
        }
    }
}