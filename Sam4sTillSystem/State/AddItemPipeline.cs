using Sam4sTillSystem.Model;
using System;
using System.Collections.Generic;

namespace Sam4sTillSystem.State
{
    public class AddItemPipeline
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ItemName { get; set; }
        public int BasePrice { get; set; }
        public int Qty { get; set; } = 1;
        public string NavMenuFrom { get; set; }
        public bool IncludeInChefsNotes { get; set; } = false;
        public Dictionary<string, SectionChanges> SectionChanges { get; set; } = new Dictionary<string, SectionChanges>();

        public int GetTotalCost()
        {
            int costOfExtras = 0;

            foreach (string key in SectionChanges.Keys)
            {
                foreach (OrderItem item in SectionChanges[key].Changes)
                {
                    costOfExtras += item.PriceInPence * item.Qty;
                }
            }

            return (BasePrice + costOfExtras) * Qty;
        }

        public int GetSingeQtyCost()
        {
            int costOfExtras = 0;

            foreach (string key in SectionChanges.Keys)
            {
                foreach (OrderItem item in SectionChanges[key].Changes)
                {
                    costOfExtras += item.PriceInPence * item.Qty;
                }
            }

            return (BasePrice + costOfExtras);
        }

        public bool AllExceptItemQuantityMatch(AddItemPipeline itemPipeline)
        {
            if (ItemName == itemPipeline.ItemName)
            {
                if (BasePrice == itemPipeline.BasePrice)
                {
                    if (SectionChanges.Keys.Count == itemPipeline.SectionChanges.Keys.Count)
                    {
                        foreach (string key in SectionChanges.Keys)
                        {
                            if (!itemPipeline.SectionChanges.ContainsKey(key))
                            {
                                return false;
                            }

                            if (itemPipeline.SectionChanges[key].Changes.Count != SectionChanges[key].Changes.Count)
                            {
                                return false;
                            }

                            foreach (OrderItem orderItem in itemPipeline.SectionChanges[key].Changes)
                            {
                                bool matchingExtraFound = false;
                                foreach (OrderItem orderItem2 in SectionChanges[key].Changes)
                                {
                                    if(orderItem.ItemName == orderItem2.ItemName && 
                                        orderItem.PriceInPence == orderItem2.PriceInPence &&
                                        orderItem.Qty == orderItem2.Qty &&
                                        orderItem.Total == orderItem2.Total &&
                                        orderItem.IncludeInChefsNotes == orderItem2.IncludeInChefsNotes)
                                    {
                                        matchingExtraFound = true;
                                    }
                                }

                                if (!matchingExtraFound)
                                {
                                    return false;
                                }
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class SectionChanges
    {
        public bool IsItemRemovals { get; set; } = false;
        public List<OrderItem> Changes { get; set; } = new List<OrderItem>();
    }
}