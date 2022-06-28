using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sam4sTillSystem.Model.ConfigurablePart
{
    public class CloseScreenConfig
    {
        public string DisplayText { get; set; } = "Are you sure\nyou want to close the application";
        public int YesFontSizeInPixels { get; set; } = 18;
        public int YesButtonHeightInPixels { get; set; } = 160;
        public int NoFontSizeInPixels { get; set; } = 18;
        public int NoButtonHeightInPixels { get; set; } = 160;
        public int AreYouSureFontSize { get; set; } = 50;
    }
}
