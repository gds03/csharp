using System;
using System.Text;
using System.Web.Mvc;

namespace VirtualNote.MVC.Helpers
{
    public static class PagerHelper
    {
        // Em cada anchor necessitamos de guardar apenas a pagina
        static String BuildAnchorElement(string text, int page, string className) {
            StringBuilder htmlString = new StringBuilder();

            htmlString.AppendFormat("<a class='{2}' href='#' data-page='{0}'>{1}</a>", page, text, className);
            htmlString.AppendLine();

            return htmlString.ToString();
        }

        static String BuildAnchorElement(string text, int page) {
            return BuildAnchorElement(text, page, String.Empty);
        }

        


        public static MvcHtmlString Pager(this HtmlHelper html,
            int currentPage,
            int pageSize,
            int totalItems,
            int numNextAnchors,
            string selectedClassName)
        {
            int totalPages = totalItems / pageSize;
            if (totalItems % pageSize > 0)
                totalPages++;

            if (totalPages <= 1)
                return MvcHtmlString.Empty;
            

            if(currentPage > totalPages)
                throw new InvalidOperationException("Invalid current page");

            StringBuilder htmlString = new StringBuilder();
            int temp;

            int seedStart = (temp = (currentPage - ( numNextAnchors/2 )) ) < 1 ? 1 : temp;

            //  Criar [Prev] se, currentPage > 1
            if(currentPage > 1) {
                htmlString.AppendLine(BuildAnchorElement("[Prev]", currentPage - 1));
            }

            // Criar 1 se, seedStart > 1
            if(seedStart > 1)
                htmlString.AppendLine(BuildAnchorElement("1", 1));

            // Criar ... se, seedStart >= 3
            if(seedStart >= 3)
                htmlString.AppendLine(BuildAnchorElement("...", seedStart));

            
            
            // Criar lista de ancoras
            int seedIdx = seedStart;
            for (int idx = 0; (idx < numNextAnchors && idx + seedStart <= totalPages); idx++, seedIdx++) 
            {
                htmlString.AppendLine(BuildAnchorElement(seedIdx.ToString(), seedIdx, currentPage == seedIdx ? selectedClassName : String.Empty));
            }




            // Criar ... se, seedIdx < totalPages && seedIdx + 1 != totalPages
            if(seedIdx < totalPages && seedIdx + 1 != totalPages)
                htmlString.AppendLine(BuildAnchorElement("...", seedIdx + 1));


            // Criar "lastId" se, seedIdx < totalPages
            if(seedIdx <= totalPages)
                htmlString.AppendLine(BuildAnchorElement(totalPages.ToString(), totalPages));


            //  Criar [Next] se, currentPage + 1 <= totalPages
            if(currentPage + 1 <= totalPages) {
                htmlString.AppendLine(BuildAnchorElement("[Next]", currentPage + 1));
            }


            return MvcHtmlString.Create(htmlString.ToString());

            //          [ 1 ]   2   3   4   5   ...     24   [Next]

            //    [Prev]  1   2   [ 3 ]  4   5   ...     24   [Next]

            //    [Prev]  1   2   3   [ 4 ]  5   6   ...     24   [Next]

            //    [Prev]  1 ...   6   7   [ 8 ]  9   10   ...     24   [Next]

            //    [Prev]  1 ...   20   21   [ 22 ]  23   24   24   [Next]
        }



        public static MvcHtmlString Pager(this HtmlHelper html,
            int currentPage,
            int pageSize,
            int totalItems,
            int numNextAnchors) {
            return Pager(html, currentPage, pageSize, totalItems, numNextAnchors, String.Empty);
        }
    }
}