using CustomComponents.Mvc.UserControls.Models.GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CustomComponents.Mvc.UserControls.Extensions;

namespace CustomComponents.Mvc.UserControls.Types.UserControls
{
    public class UserControls
    {
        public const string GRIDVIEW_NAME = "______GridView";
    }


    public class UserControls<TModel> : UserControls
    {
        public HtmlHelper<TModel> Helper { get; set; }

        public UserControls(HtmlHelper<TModel> html)
        {
            if (html == null)
                throw new ArgumentNullException("html");

            this.Helper = html;
        }




        /// <summary>
        ///     Generates a gridview for the current selector, and optionally you can pass formId for the grid to peak form values and send them when paginating and sorting.
        ///     NOTE: When you use formID parameter, the grid posts that key when paginating and ordering, so you must put 'formId' key on the action that executes the query.
        /// </summary>
        public MvcHtmlString GridViewFor(Expression<Func<TModel, GridContext>> gridContextSelector, string formId = null)
        {
            if (formId != null && formId == "")
                throw new ArgumentNullException("formId cannot be empty");

            return Helper.Partial(gridContextSelector, GRIDVIEW_NAME, new { @formId = formId });
        }
    }
}