using System;

namespace Sam4sTillSystem.Helpers
{
    public static class SqliteHelper
    {
        public static int ConvertDateToInteger(DateTime dateTime)
        {
            return (dateTime.Year * 10000) + (dateTime.Month * 100) + dateTime.Day;
        }
        public static int ConvertTimeToInteger(DateTime dateTime)
        {
            return (dateTime.Hour * 10000) + (dateTime.Minute * 100) + dateTime.Second;
        }
    }
}
