using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sam4sTillSystem.Helpers
{
    public static class MenuHelper
    {
        public static ProductButton GetProductButton(int productButtonId, MenuSettings menuSettings = null)
        {
            if (menuSettings != null)
            {
                return menuSettings.ProductButtons.Where(x => x.ProductButtonId == productButtonId).First();
            }

            return GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == productButtonId).First();
        }

        public static ProductScreen GetProductScreen(int productScreenId)
        {
            return GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == productScreenId).First();
        }

        public static ProductScreenControl GetProductScreenControl(int productScreenControlId)
        {
            return GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == productScreenControlId).First();
        }

        public static int[] GetAdditions()
        {
            return GlobalData.MenuSettings.GenericExtraScreenControls;
        }
    }
}
