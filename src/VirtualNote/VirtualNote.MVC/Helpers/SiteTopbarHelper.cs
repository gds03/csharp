using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualNote.MVC.Helpers
{
    public static class SiteTopbarHelper
    {
        public static MvcHtmlString CreateTopbar(this HtmlHelper html,
            string classCurrent)
        {
            return MvcHtmlString.Create(GenerateFirstLevelMenu("", classCurrent, html.ViewContext));
        }

        public static MvcHtmlString CreateTopbar(this HtmlHelper html,
            string id,
            string classCurrent)
        {
            return MvcHtmlString.Create(GenerateFirstLevelMenu(id, classCurrent, html.ViewContext));   
        }


        static String GenerateFirstLevelMenu(String id, String classCurrent, ViewContext viewContext)
        {
            //
            // È sempre chamado
            var ul = new TagBuilder("ul");
            
            if(!String.IsNullOrEmpty(id))
                ul.MergeAttribute("id", id);

            foreach (SiteMapNode child in SiteMap.RootNode.ChildNodes)
            {
                TagBuilder li = new TagBuilder("li");

                li.MergeAttribute("class", "nav_icon");

                // Se sou o current então atribuo class
                if (CheckIfCurrent(child, viewContext)) {
                    li.AddCssClass(classCurrent);
                }

                TagBuilder a = new TagBuilder("a");
                a.MergeAttribute("href", child.Url);

                TagBuilder span = new TagBuilder("span");
                span.MergeAttribute("class", ClassHelper.GetValue(child.Title));    // Helper method para atribuir a class

                a.InnerHtml = span.ToString() + child.Title;
                li.InnerHtml = a.ToString();

                if (child.HasChildNodes && TopbarDrawIgnore.IsToDraw(child.Title))
                {
                    //
                    TagBuilder div = new TagBuilder("div");
                    div.MergeAttribute("class", "nav_menu");

                    TagBuilder innerUl = new TagBuilder("ul");
                    foreach (SiteMapNode innerChild in child.ChildNodes) 
                    {

                        // Se sou o current então atribuo class
                        if (CheckIfCurrent(innerChild, viewContext)) {
                            li.AddCssClass(classCurrent);
                        }

                        TagBuilder innerLi = new TagBuilder("li");
                        TagBuilder innerAnchor = new TagBuilder("a");
                        innerAnchor.MergeAttribute("href", innerChild.Url);
                        innerAnchor.SetInnerText(innerChild.Title);

                        innerLi.InnerHtml = innerAnchor.ToString();
                        innerUl.InnerHtml += innerLi.ToString();
                    }
                    div.InnerHtml = innerUl.ToString();
                    li.InnerHtml += div.ToString();
                }

                ul.InnerHtml += li.ToString();
            }

            return ul.ToString();
        }
        static bool CheckIfCurrent(SiteMapNode node, ViewContext viewContext){
            return viewContext.RouteData.GetRequiredString("controller") == node["controller"];
        }
    }

    public static class TopbarDrawIgnore
    {
        private static readonly String[] IgnoreTitles = new[] { "configurations" };



        public static bool IsToDraw(String title) {
            return IgnoreTitles.Contains(title.ToLower().Trim());
        }
    }

    public static class ClassHelper
    {
        static readonly IDictionary<string, string> Container = new Dictionary<string, string>();

        static ClassHelper()
        {
            Container.Add("home", "ui-icon ui-icon-home");
            Container.Add("configurations", "ui-icon ui-icon-gripsmall-diagonal-se");
            Container.Add("issues", "ui-icon ui-icon-signal");
        }

        public static string GetValue(string key)
        {
            return Container[key.Trim().ToLower()];
        }
    }




//    <ul id="nav">
//    <li class="nav_current nav_icon">
//        <a href="/Home">
//            <span class="ui-icon ui-icon-home"></span>
//            Home
//        </a>
//    </li>

//    <li class="nav_dropdown nav_icon">
//        <a href="">
//            <span class="ui-icon ui-icon-gripsmall-diagonal-se"></span>
//            Configurations
//        </a>
	
//        <div class="nav_menu">			
//            <ul>
//                <li><a href="/Configurations/Members">Members</a></li>	
//                <li><a href="/Configurations/Clients">Clients</a></li>	
//                <li><a href="/Configurations/Projects">Projects</a></li>						
//            </ul>
		
//        </div>
//    </li>

//    <li class="nav_icon">
//        <a href="/Issues"><span class="ui-icon ui-icon-signal"></span>Issues</a>
//    </li>
//</ul>
}