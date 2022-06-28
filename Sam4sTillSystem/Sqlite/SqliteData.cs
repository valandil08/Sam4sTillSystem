using Newtonsoft.Json;
using Sam4sTillSystem.Data;
using Sam4sTillSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Sam4sTillSystem.Sqlite.Model;
using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using Sam4sTillSystem.Data.JsonObject;

namespace Sam4sTillSystem.Sqlite
{
    public static partial class SqliteData
    {
        private const string SqliteDbFileName = "TillSystemData.db";

        private static SQLiteConnection GetSqliteConnection()
        {
            return new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; ");
        }

        public static bool CreateDatabaseIfNotExists()
        {
            try
            {
                if (!File.Exists(SqliteDbFileName))
                {
                    SQLiteConnection.CreateFile(SqliteDbFileName);
                }

                // create sql table if not exist
                using (SQLiteConnection conn = GetSqliteConnection())
                {
                    conn.Open();

                    // use year + month + day for dateCreated
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = @"create table if not exists OrderHistory
                        (
                            orderIdentifier text,  
                            menuIdentifier text,  
                            paymentType text,                         
                            dataAsJson text,
                            totalCostInPence integer,
                            vatRate real,
                            dateCreated integer,
                            timeCreated integer,
                            syned integer
                        );  

                        create table if not exists Menus
                        (
                            menuIdentifier text,
                            dataAsJson text,
                            dateCreated integer
                        );

                        create table if not exists NoSaleActivations
                        (
                            dateCreated integer, 
                            timeCreated integer
                        );
                    ";
                    command.ExecuteNonQuery();

                    conn.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static void SaveOrder(OrderInfo orderInfo, double vatRate, bool isCash)
        {
            if (GlobalData.TestModeEnabled)
            {
                return;
            }

            orderInfo.MenuIdentifier = Sqlite.SqliteData.GetIdentiferForMenuConfig();

            string dataAsJson = JsonConvert.SerializeObject(orderInfo);
            string orderIdentifier = orderInfo.Identifier.ToString();
            string menuIdentifier = null;
            int totalCostInPence = orderInfo.GetTotalCostInPence();
            int dateCreated = SqliteHelper.ConvertDateToInteger(DateTime.Now);
            int timeCreated = SqliteHelper.ConvertTimeToInteger(DateTime.Now);

            using (SQLiteConnection conn = GetSqliteConnection())
            {
                conn.Open();

                // use year + month + day for dateCreated
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = @"INSERT INTO OrderHistory
                        (
                            orderIdentifier,  
                            menuIdentifier,  
                            paymentType,
                            dataAsJson,
                            totalCostInPence,
                            vatRate,
                            dateCreated,
                            timeCreated,
                            syned
                        )  

                        VALUES
                        (
                            @orderIdentifier,  
                            @menuIdentifier,  
                            @paymentType,                        
                            @dataAsJson,
                            @totalCostInPence,
                            @vatRate,
                            @dateCreated,
                            @timeCreated,
                            0
                        )
                    ";

                command.Parameters.Add(new SQLiteParameter("@orderIdentifier", orderIdentifier));
                command.Parameters.Add(new SQLiteParameter("@menuIdentifier", menuIdentifier));
                command.Parameters.Add(new SQLiteParameter("@paymentType", isCash ? "cash" : "card"));
                command.Parameters.Add(new SQLiteParameter("@dataAsJson", dataAsJson));
                command.Parameters.Add(new SQLiteParameter("@totalCostInPence", totalCostInPence));
                command.Parameters.Add(new SQLiteParameter("@vatRate", vatRate));
                command.Parameters.Add(new SQLiteParameter("@dateCreated", dateCreated));
                command.Parameters.Add(new SQLiteParameter("@timeCreated", timeCreated));
                command.ExecuteNonQuery();

                conn.Close();
            }
        }


        public static bool RecordNoSale()
        {
            if (GlobalData.TestModeEnabled)
            {
                return true;
            }

            try
            {
                string dateCreated = DateTime.Now.Year.ToString("0000") + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00");
                string timeCreated = DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
                {
                    conn.Open();

                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO NoSaleActivations(dateCreated, timeCreated) values (" + dateCreated + "," + timeCreated + ")";
                    command.ExecuteNonQuery();

                    conn.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<int> GetListOfNoSaleActivations(DateTime dateTime)
        {
            string dateCreated = DataHelper.ConvertDateToString(dateTime);

            List<int> noSaleActivations = new List<int>();

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
            {

                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = "SELECT timeCreated FROM NoSaleActivations WHERE dateCreated = " + dateCreated;

                conn.Open();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        noSaleActivations.Add(int.Parse(reader["timeCreated"].ToString()));
                    }
                }


                conn.Close();
            }

            return noSaleActivations;
        }

        public static DailyStats GetDailyStats(DateTime dateTime)
        {
            try
            {
                DailyStats stats = new DailyStats();

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
                {
                    conn.Open();

                    string dateCreated = dateTime.Year.ToString("0000") + dateTime.Month.ToString("00") + dateTime.Day.ToString("00");
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT dataAsJson FROM OrderHistory WHERE dateCreated = " + dateCreated;

                    List<OrderInfo> orders = new List<OrderInfo>();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(JsonConvert.DeserializeObject<OrderInfo>(reader["dataAsJson"].ToString()));
                        }
                    }

                    foreach (MenuCategory category in GlobalData.MenuSettings.Categories.OrderBy(x => x.Text))
                    {
                        stats.SectionSales.Add(category.Text, 0);
                    }

                    stats.SectionSales.Add("Unknown", 0);


                    foreach (OrderInfo orderInfo in orders)
                    {
                        MenuSettings menuSettings = GetMenuSettingsForIdentifier(orderInfo.MenuIdentifier);

                        try
                        {
                            int totalInPence = orderInfo.GetTotalCostInPence(menuSettings);
                            int totalDiscountInPence = orderInfo.GetTotalDiscountInPence(menuSettings);

                            stats.JustEatSales += orderInfo.DestinationNumber == 88888 ? totalInPence : 0;
                            stats.JustEatSalesDiscount += orderInfo.DestinationNumber == 88888 ? totalDiscountInPence : 0;
                            stats.OnSiteSales += orderInfo.DestinationNumber < 1000 ? totalInPence : 0;
                            stats.OnSiteSalesDiscount += orderInfo.DestinationNumber < 1000 ? totalDiscountInPence : 0;
                            stats.TakeAwaySales += orderInfo.DestinationNumber == 99999 ? totalInPence : 0;
                            stats.TakeAwaySalesDiscount += orderInfo.DestinationNumber == 99999 ? totalDiscountInPence : 0;
                            stats.TotalCardSales += orderInfo.paymentMethod == Enum.PaymentMethod.Card ? totalInPence : 0;
                            stats.TotalCardSalesDiscount += orderInfo.paymentMethod == Enum.PaymentMethod.Card ? totalDiscountInPence : 0;
                            stats.TotalCashSales += orderInfo.paymentMethod == Enum.PaymentMethod.Cash ? totalInPence : 0;
                            stats.TotalCashSalesDiscount += orderInfo.paymentMethod == Enum.PaymentMethod.Cash ? totalDiscountInPence : 0;
                            stats.TotalDiscount += totalDiscountInPence;
                            stats.TotalManualEntry += orderInfo.GetTotalManualEntryInPence(menuSettings);
                            stats.TotalRefund += orderInfo.GetTotalRefundInPence(menuSettings);
                            stats.TotalSales += totalInPence;
                            stats.TotalSalesDiscount += totalDiscountInPence;
                            stats.TotalVat += orderInfo.GetTotalVatInPence(menuSettings);

                            foreach (OrderItemInfo orderInfoItem in orderInfo.OrderItems)
                            {
                                if (orderInfoItem.ProductButtonId < 0)
                                {
                                    continue;
                                }
                                ProductButton button = MenuHelper.GetProductButton(orderInfoItem.ProductButtonId, menuSettings);

                                MenuCategory category = menuSettings.Categories.Where(x => x.MenuCategoryId == button.MenuCategoryId).First();

                                stats.SectionSales[category.Text] += orderInfoItem.GetTotalCostInPence(menuSettings);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    conn.Close();
                }

                return stats;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetIdentiferForMenuConfig()
        {
            try
            {
                string identifer = null;

                string dataAsJson = JsonConvert.SerializeObject(GlobalData.MenuSettings);

                using (SQLiteConnection conn = GetSqliteConnection())
                {

                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = " SELECT 1 as 'Exists' FROM Menus WHERE dataAsJson = @dataAsJson";
                    command.Parameters.Add(new SQLiteParameter("@dataAsJson", dataAsJson));

                    conn.Open();

                    bool recordFound = false;

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int exists = int.Parse(reader["Exists"].ToString());

                            if (exists == 1)
                            {
                                recordFound = true;
                            }
                        }
                    }


                    if (recordFound)
                    {
                        command.CommandText = " SELECT menuIdentifier FROM Menus WHERE dataAsJson = @dataAsJson";
                        command.Parameters.Add(new SQLiteParameter("@dataAsJson", dataAsJson));
                        
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                identifer = reader["menuIdentifier"].ToString();
                            }
                        }
                    }
                    else
                    {
                        identifer = Guid.NewGuid().ToString();

                        command.CommandText = " INSERT INTO Menus (menuIdentifier, dataAsJson, dateCreated) values(@menuIdentifier, @dataAsJson, @dateCreated)";
                        command.Parameters.Add(new SQLiteParameter("@menuIdentifier", identifer));
                        command.Parameters.Add(new SQLiteParameter("@dataAsJson", dataAsJson));
                        command.Parameters.Add(new SQLiteParameter("@dateCreated", SqliteHelper.ConvertDateToInteger(DateTime.Now)));
                        command.ExecuteNonQuery();
                    }

                    conn.Close();
                }

                return identifer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static MenuSettings GetMenuSettingsForIdentifier(string identifier)
        {
            try
            {
                using (SQLiteConnection conn = GetSqliteConnection())
                {

                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = " SELECT dataAsJson FROM Menus WHERE menuIdentifier = @menuIdentifier";
                    command.Parameters.Add(new SQLiteParameter("@menuIdentifier", identifier));

                    conn.Open();

                    string dataAsJson = null;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataAsJson = reader["dataAsJson"].ToString();
                        }
                    }
                    conn.Close();

                    return JsonConvert.DeserializeObject<MenuSettings>(dataAsJson);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
