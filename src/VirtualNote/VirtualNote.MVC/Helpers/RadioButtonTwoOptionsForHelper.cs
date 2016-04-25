using System;
using System.Web.Mvc;
using System.Linq.Expressions;
using VirtualNote.Common;

namespace VirtualNote.MVC.Helpers
{
    public static class RadioButtonTwoOptionsForHelper
    {
        private static MvcHtmlString _RadioButtonTwoOptionsFor<TModel>(HtmlHelper<TModel> helper,
                                                                     Expression<Func<TModel, bool>> property,
                                                                     String positiveText, String negativeText,
                                                                     bool defaultValueForEmptyModel,
                                                                     bool forcedDefault)
        {
            TagBuilder divExtern = new TagBuilder("div");
            divExtern.MergeAttribute("class", "controlset-pad");
            bool state;

            if (forcedDefault)
            {
                state = defaultValueForEmptyModel;
            }
            else
            {
                TModel model = helper.ViewData.Model;

                if (model == null) {
                    state = defaultValueForEmptyModel;
                } else {
                    try {
                        state = property.Compile()(model);
                    } catch (Exception) {
                        state = defaultValueForEmptyModel;
                    }
                }
            }

            String name = property.GetMemberInfo().Member.Name;

            // 
            // Positive
            TagBuilder input = new TagBuilder("input");
            input.MergeAttribute("type", "radio");
            input.MergeAttribute("name", name);
            input.MergeAttribute("value", "true");
            if (state)
            {
                input.MergeAttribute("checked", "");
            }

            TagBuilder label = new TagBuilder("label");
            label.SetInnerText(positiveText);

            TagBuilder br = new TagBuilder("br");

            divExtern.InnerHtml = (input.ToString(TagRenderMode.SelfClosing) + label.ToString() + br.ToString(TagRenderMode.SelfClosing));

            //
            // Negative
            input = new TagBuilder("input");
            input.MergeAttribute("type", "radio");
            input.MergeAttribute("name", name);
            input.MergeAttribute("value", "false");
            if (!state)
            {
                input.MergeAttribute("checked", "");
            }

            label = new TagBuilder("label");
            label.SetInnerText(negativeText);

            br = new TagBuilder("br");

            divExtern.InnerHtml += (input.ToString(TagRenderMode.SelfClosing) + label.ToString() + br.ToString(TagRenderMode.SelfClosing));
            return MvcHtmlString.Create(divExtern.ToString());




            //<div class="controlset-pad">
      
            //    <input type="radio" id="radio1" name="Enabled" value="" /> 
            //    <label for="radio1"> PositiveText </label>
            //    <br />

            //    <input type="radio" id="radio2" name="Enabled" value="" /> 
            //    <label for="radio2"> NegativeText </label>
            //    <br />

            //</div>
        }

        /// <summary>
        ///     Constroi duas radio buttons com o name e id de property, com o positiveText a valer 1, e com negativeText a valer 0
        ///     e caso o TModel não exista na View é atribuido o default value de defaultValueForEmptyModel, caso exista fica com o valor de TModel
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="property"></param>
        /// <param name="positiveText"></param>
        /// <param name="negativeText"></param>
        /// <param name="defaultValueForEmptyModel"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonTwoOptionsFor<TModel>(this HtmlHelper<TModel> helper,
                                                                     Expression<Func<TModel, bool>> property,
                                                                     String positiveText, String negativeText,
                                                                     bool defaultValueForEmptyModel)
        {
            return _RadioButtonTwoOptionsFor(helper, property, positiveText, negativeText, defaultValueForEmptyModel,
                                             false);
        }



        /// <summary>
        ///     Constroi duas radio buttons com o name e id de property, com o positiveText a valer 1, e com negativeText a valer 0
        ///     e caso o TModel não exista na View é atribuido o default value de defaultValueForEmptyModel, caso exista fica com o valor de forcedDefault
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="property"></param>
        /// <param name="positiveText"></param>
        /// <param name="negativeText"></param>
        /// <param name="defaultValueForEmptyModel"></param>
        /// <param name="forcedDefault"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonTwoOptionsFor<TModel>(this HtmlHelper<TModel> helper,
                                                                     Expression<Func<TModel, bool>> property,
                                                                     String positiveText, String negativeText,
                                                                     bool defaultValueForEmptyModel,
                                                                     bool forcedDefault)
        {
            return _RadioButtonTwoOptionsFor(helper, property, positiveText, negativeText, defaultValueForEmptyModel,
                                             forcedDefault);
        }
    }
}