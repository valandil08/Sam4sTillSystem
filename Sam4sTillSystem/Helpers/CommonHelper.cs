using Sam4sTillSystem.Model;

namespace Sam4sTillSystem.Helpers
{
    public class CommonHelper
    {
        public static void ClearCashAmount(MainWindow window, PageState state)
        {
            state.PaymentAmountText = "";
            window.PaymentInput.Text = "";
        }
    }
}
