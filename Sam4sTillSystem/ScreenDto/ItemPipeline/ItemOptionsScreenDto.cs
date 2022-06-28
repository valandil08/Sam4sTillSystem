using System;
using System.Collections.Generic;

namespace Sam4sTillSystem.ScreenDto.ItemPipeline
{
    public class ItemOptionsScreenDto
    {
        public Guid orderPartIdentifier = Guid.NewGuid();

        public bool IsEditMode { get; set; } = false;

        public int Quantity { get; set; } = 1;
    }
}
