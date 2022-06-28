using System;

namespace Sam4sTillSystem.State
{
    public class YesNoPopupModel
    {
        public Action NoPressedAction { get; set; } = null;
        public Action YesPressedAction { get; set; } = null;
    }
}
