using CustomComponents.Mvc.UserControls.Types.ViewEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomComponents.Mvc.UserControls.Configuration
{
    public static class UserControlsConfig
    {
        public static void Register()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new UserControlsViewEngine());
        }
    }
}