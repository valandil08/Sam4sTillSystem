using Microsoft.PointOfService;

namespace Sam4sTillSystem.HardwareFunctions
{
    public static class CashDrawer
    {
        private static Microsoft.PointOfService.CashDrawer cashDrawer = null;

        public static bool Open()
        {
            if (cashDrawer == null)
            {
                try
                {
                    PosExplorer explorer = new PosExplorer();
                    DeviceInfo ObjDevicesInfo = explorer.GetDevice("CashDrawer", "Cash Drawer");
                    cashDrawer = (Microsoft.PointOfService.CashDrawer)explorer.CreateInstance(ObjDevicesInfo);
                    cashDrawer.Open();
                    cashDrawer.Claim(1000);
                    cashDrawer.DeviceEnabled = true;
                }
                catch
                {
                    return false;
                }
            }


            try
            {
                if (cashDrawer.DrawerOpened == false)
                {
                    cashDrawer.OpenDrawer();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
