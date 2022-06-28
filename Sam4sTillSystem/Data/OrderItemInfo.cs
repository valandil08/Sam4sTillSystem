using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using Sam4sTillSystem.Data.OrderOptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sam4sTillSystem.Data
{
    public class OrderItemInfo
    {
        // used to detect if new item is being added or existing item is edited
        public Guid Identifier { get; } = Guid.NewGuid();

        public List<string> ChefsPrintersToSendTo { get; set; } = new List<string>();
            
        public int ProductButtonId { get; set; }

        public int Quantity { get; set; } = 1;

        public int BasePriceInPence { get; set; } = 0;

        // used for edit functionality
        public int[] ProductScreenIds { get; set; } = new int[0];

        // <ScreenId, ControlQuantityValues>
        // index of array matches index in controlIds array for screen
        public Dictionary<int, int[]> ScreenValues = new Dictionary<int, int[]>();

        public OrderItemInfo()
        {
            Identifier = Guid.NewGuid();
        }

        public int GetAdjustmentFee(MenuSettings menuSettings = null)
        {
            if (menuSettings == null)
            {
                menuSettings = GlobalData.MenuSettings;
            }

            int additionTotalInPence = 0;
            int removalTotalInPence = 0;

            for (int i = ProductScreenIds.Length; i < ScreenValues.Count(); i++)
            {
                bool isGenericExtras = i == ProductScreenIds.Length;
                bool isAdjustments = i == ProductScreenIds.Length + 1;

                int[] controlValues = new int[0];

                if (ScreenValues.ContainsKey(i))
                {
                    controlValues = ScreenValues[i];
                }

                for (int j = 0; j < controlValues.Length; j++)
                {

                    int controlId = 0;

                    if (isGenericExtras)
                    {
                        controlId = menuSettings.GenericExtraScreenControls[j];
                    }
                    else if (isAdjustments)
                    {
                        controlId = menuSettings.ProductButtons.Where(x => x.ProductButtonId == ProductButtonId).First().AdjustmentControlIds[j];
                    }
                    else
                    {
                        int screenId = ProductScreenIds[i];
                        controlId = menuSettings.ProductScreens.Where(x => x.ProductScreenId == screenId).First().ProductScreenControlIds[j];
                    }

                    ProductScreenControl control = menuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == controlId).First();

                    int difference;

                    if (control.IsQuantitySetter)
                    {
                        difference = controlValues[j] - control.StartingQuantity;
                    }
                    else
                    {
                        difference = controlValues[j] - control.StartingQuantity;
                    }

                    if (difference != 0)
                    {
                        int totalInPence = difference > 0 ? control.CostPerExtraItemInPence * difference : -control.DiscountPerRemovedItemInPence * difference;

                        if (isGenericExtras)
                        {
                            additionTotalInPence += totalInPence;
                        }
                        else if (isAdjustments)
                        {
                            removalTotalInPence += totalInPence;
                        }
                    }
                }
            }


            if ((additionTotalInPence + removalTotalInPence) > 0)
            {
                return additionTotalInPence + removalTotalInPence;
            }
            else
            {
                return 0;
            }

        }

        public int GetTotalCostInPence(MenuSettings menuSettings = null)
        {
            return GetTotalCostEachInPence(menuSettings) * Quantity;
        }

        public int GetTotalCostEachInPence(MenuSettings menuSettings = null)
        {
            if (menuSettings == null)
            {
                menuSettings = GlobalData.MenuSettings;
            }

            int orderTotalInPence = 0;

            for (int i = 0; i < ProductScreenIds.Count(); i++)
            {
                int[] controlValues = new int[0];

                if (ScreenValues.ContainsKey(i))
                {
                    controlValues = ScreenValues[i];
                }

                for (int j = 0; j < controlValues.Length; j++)
                {
                    int screenId = ProductScreenIds[i];
                    int controlId = menuSettings.ProductScreens.Where(x => x.ProductScreenId == screenId).First().ProductScreenControlIds[j];

                    ProductScreenControl control = menuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == controlId).First();

                    int difference;

                    if (control.IsQuantitySetter)
                    {
                        difference = controlValues[j] - control.StartingQuantity;
                    }
                    else
                    {
                        difference = controlValues[j] - control.StartingQuantity;
                    }

                    if (difference != 0)
                    {
                        int totalInPence = difference > 0 ? control.CostPerExtraItemInPence * difference : -control.DiscountPerRemovedItemInPence * difference;

                        orderTotalInPence += totalInPence;
                    }
                }
            }

            orderTotalInPence += BasePriceInPence;
            orderTotalInPence += GetAdjustmentFee();

            return orderTotalInPence;
        }        
    }
}
