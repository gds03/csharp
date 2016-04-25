using System.Web.Mvc;
using VirtualNote.Kernel;

namespace VirtualNote.MVC.Extensions
{
    public static class ViewPageExtensions
    {
        public static string GetClassForStateValue(this WebViewPage page, StateEnum s) {
            switch (s) {
                case StateEnum.Waiting:
                return "ticket open";

                case StateEnum.InResolution:
                return "ticket responded";

                case StateEnum.Terminated:
                return "ticket closed";
            }
            return "";
        }
    }
}