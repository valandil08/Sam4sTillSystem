using Newtonsoft.Json;
using Sam4sTillSystem.Data.JsonObject;
using Sam4sTillSystem.Data.JsonObject.SiteSetting;
using Sam4sTillSystem.Model;
using Sam4sTillSystem.Model.ConfigurablePart;
using System;
using System.IO;

namespace Sam4sTillSystem.Helpers
{
    public static class DataHelper
    {
        public static string ConvertIntToMoneyString(int priceInPence)
        {
            string prefix = "";

            if (priceInPence < 0)
            {
                prefix = "-";
                priceInPence *= -1;
            }
            if (priceInPence < 10)
            {
                return prefix + "£0.0" + priceInPence;
            }

            if (priceInPence < 100)
            {
                return prefix + "£0." + priceInPence;
            }
            string priceInPenceString = priceInPence.ToString();


            return prefix + "£" + priceInPenceString.Insert(priceInPenceString.Length - 2, ".");
        }

        public static string ConvertLongToDateTimeString(long dateTime)
        {
            string dateTimeString = dateTime.ToString();
            string year = dateTimeString.Substring(0, 4);
            string month = dateTimeString.Substring(4, 2);
            string day = dateTimeString.Substring(6, 2);
            string hour = dateTimeString.Substring(8, 2);
            string minuite = dateTimeString.Substring(10, 2);
            string second = dateTimeString.Substring(12);

            return day + "/" + month + "/" + year + " " + hour + ":" + minuite + ":" + second;
        }

        public static string ConvertIntToTimeString(int time)
        {
            string timeString = time.ToString();

            if (timeString.Length == 7)
            {
                timeString = ("0" + timeString);
            }

            string hour = timeString.Substring(0, 2);
            string minuite = timeString.Substring(2, 2);
            string second = timeString.Substring(4);

            return hour + ":" + minuite + ":" + second;
        }

        public static string ConvertDateToString(DateTime date)
        {
            return date.Year.ToString("0000") + date.Month.ToString("00") + date.Day.ToString("00");
        }

        public static void UpdateOrderDiscount(MainWindow window, PageState state)
        {
            window.OrderDiscountTextBlock.Text = GlobalData.OrderInfo.PercentageBasedOrderDiscount + "%";
        }

        public static int GetOpeningTime(DateTime date, OpeningHoursConfig openingHours)
        {
            int openingHour = 0;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    openingHour = openingHours.MondayStart;
                    break;

                case DayOfWeek.Tuesday:
                    openingHour = openingHours.TuesdayStart;
                    break;

                case DayOfWeek.Wednesday:
                    openingHour = openingHours.WednesdayStart;
                    break;

                case DayOfWeek.Thursday:
                    openingHour = openingHours.ThursdayStart;
                    break;

                case DayOfWeek.Friday:
                    openingHour = openingHours.FridayStart;
                    break;

                case DayOfWeek.Saturday:
                    openingHour = openingHours.SaturdayStart;
                    break;

                case DayOfWeek.Sunday:
                    openingHour = openingHours.SundayStart;
                    break;
            }

            return openingHour * 100;
        }

        public static int GetClosingTime(DateTime date, OpeningHoursConfig openingHours)
        {
            int closingHour = 0;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    closingHour = openingHours.MondayEnd;
                    break;

                case DayOfWeek.Tuesday:
                    closingHour = openingHours.TuesdayEnd;
                    break;

                case DayOfWeek.Wednesday:
                    closingHour = openingHours.WednesdayEnd;
                    break;

                case DayOfWeek.Thursday:
                    closingHour = openingHours.ThursdayEnd;
                    break;

                case DayOfWeek.Friday:
                    closingHour = openingHours.FridayEnd;
                    break;

                case DayOfWeek.Saturday:
                    closingHour = openingHours.SaturdayEnd;
                    break;

                case DayOfWeek.Sunday:
                    closingHour = openingHours.SundayEnd;
                    break;
            }

            return closingHour * 100;
        }

        public static HardwareSettings GetHardwareSettings()
        {
            string fileData = File.ReadAllText(@"C:\AppSettings\HardwareSettings.json");

            return JsonConvert.DeserializeObject<HardwareSettings>(fileData);
        }
    }
}
