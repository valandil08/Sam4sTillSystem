using Newtonsoft.Json;
using Sam4sTillSystem.Enum;
using Sam4sTillSystem.Helpers;
using Sam4sTillSystem.Model;
using Sam4sTillSystem.Model.FileModel;
using System;
using System.Collections.Generic;
using Sam4sTillSystem.Data.Model;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sam4sTillSystem.MainWindow;
using Sam4sTillSystem.State;

namespace Sam4sTillSystem.Data
{
    public static class SqliteData
    {
        private const string SqliteDbFileName = "OrderToSync.db";
        /*
        public static bool CreateDatabaseIfNotExists()
        {
            try
            {

                if (!File.Exists(SqliteDbFileName))
                {
                    SQLiteConnection.CreateFile(SqliteDbFileName);
                }

                // create sql table if not exist
                using (SQLiteConnection conn = new SQLiteConnection("Data Source="+ SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
                {
                    conn.Open();

                    // use year + month + day for dateCreated
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = "create table if not exists DataToSync (uniqueId text, dataSyncedWithCloud integer, dateCreated integer, isCashSale integer, orderType text, dataAsJson text)";
                    command.ExecuteNonQuery();

                    command = conn.CreateCommand();
                    command.CommandText = "create table if not exists NoSaleActivations (dateCreated integer, timeCreated integer)";
                    command.ExecuteNonQuery();

                    command = conn.CreateCommand();
                    command.CommandText = "create table if not exists ConfigBackup (config text)";
                    command.ExecuteNonQuery();

                    command = conn.CreateCommand();
                    command.CommandText = "create table if not exists UpdatesApplied (updateName text)";
                    command.ExecuteNonQuery();
                    
                    conn.Close();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool UpdateDatabase()
        {
            KeyValuePair<string, string>[] updates =
            {
                new KeyValuePair<string, string>("AddOrderTypeColumnToDataToSync","ALTER TABLE DataToSync ADD COLUMN orderType text")
            };
            if (GlobalData.TestModeEnabled)
            {
                return true;
            }

            try
            {
                for (int i = 0; i < updates.Length; i++)
                {
                    KeyValuePair<string, string> update = updates[0];

                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
                    {
                        conn.Open();

                        SQLiteCommand command = conn.CreateCommand();
                        command.CommandText = "SELECT COUNT(1) FROM UpdatesApplied WHERE updateName = '" + update.Key + "'";
                        bool applied = int.Parse(command.ExecuteScalar().ToString()) > 0;

                        if (!applied)
                        {
                            command = conn.CreateCommand();
                            command.CommandText = "INSERT INTO UpdatesApplied (updateName) VALUES('"+ update.Key + "'); " + update.Value;
                            command.ExecuteNonQuery();
                        }

                        conn.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool LogOrder(PaymentMethod paymentMethod, OrderType orderType, CurrentOrder order)
        {
            if (GlobalData.TestModeEnabled)
            {
                return true;
            }

            try
            {
                string orderJson = JsonConvert.SerializeObject(order);
                byte[] array = Encoding.UTF8.GetBytes(orderJson);
                string arrayJson = JsonConvert.SerializeObject(array);

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
                {
                    string guid = Guid.NewGuid().ToString();
                    string dateCreated = DateTime.Now.Year.ToString("0000") + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00");
                    int cashSale = paymentMethod == PaymentMethod.Cash ? 1 : 0;
                    string orderTypeText = orderType.ToString();

                    conn.Open();

                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = "insert into DataToSync (uniqueId, dataSyncedWithCloud, dateCreated, isCashSale, orderType, dataAsJson) VALUES ('" + guid + "', 0, " + dateCreated + ", " + cashSale + ",'" + orderTypeText + "','" + arrayJson + "')";
                    command.ExecuteNonQuery();

                    conn.Close();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool LogNoSale()
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
        
        */

        public static DailyStats GetDailyStats(DateTime dateTime, List<OrderSection> orderSections)
        {
            try
            {
                DailyStats stats = new DailyStats();

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
                {
                    conn.Open();

                    string dateCreated = dateTime.Year.ToString("0000") + dateTime.Month.ToString("00") + dateTime.Day.ToString("00");
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT isCashSale, orderType, dataAsJson FROM DataToSync WHERE dateCreated = " + dateCreated;

                    List<DailyStatsRow> dailyStats = new List<DailyStatsRow>();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // convert query into list
                            DailyStatsRow statsRow = new DailyStatsRow();
                            statsRow.IsCashSale = reader["isCashSale"].ToString() == "1";
                            statsRow.OrderType = (OrderType) System.Enum.Parse(typeof(OrderType), reader["orderType"].ToString(), true);


                            byte[] data = JsonConvert.DeserializeObject<byte[]>(reader["dataAsJson"].ToString());
                            string dataAsJson = Encoding.UTF8.GetString(data);

                            CurrentOrder order = JsonConvert.DeserializeObject<CurrentOrder>(dataAsJson);
                            statsRow.Items = order.Items;
                            statsRow.OrderDiscountInPercent = order.OrderDiscountInPercent;

                            dailyStats.Add(statsRow);
                        }
                    }

                    foreach (OrderSection section in orderSections)
                    {
                        stats.SectionSales.Add(section.SectionName, 0);
                        stats.SectionSales.Add(section.SectionName+"Discount", 0);
                    }

                    stats.SectionSales.Add("Unknown", 0);


                    foreach (DailyStatsRow row in dailyStats)
                    {
                        bool isCashSale = row.IsCashSale;

                        foreach (AddItemPipeline orderItem in row.Items)
                        {
                            bool itemLogged = false;

                            int amountBeingAdded = (orderItem.GetTotalCost() * (100 - row.OrderDiscountInPercent)) / 100;
                            int amountDiscounted = orderItem.GetTotalCost() - amountBeingAdded;

                            if (amountDiscounted > 0)
                            {
                                stats.TotalDiscount += amountDiscounted;
                            }

                            if (isCashSale)
                            {
                                stats.TotalCashSales += amountBeingAdded;
                            }
                            else
                            {
                                stats.TotalCardSales += amountBeingAdded;
                            }

                            switch (row.OrderType)
                            {
                                case OrderType.OnSite:
                                    stats.OnSiteSales += amountBeingAdded;
                                    stats.OnSiteSalesDiscount += amountDiscounted;
                                    break;

                                case OrderType.JustEat:
                                    stats.JustEatSales += amountBeingAdded;
                                    stats.JustEatSales += amountDiscounted;
                                    break;

                                case OrderType.Takeaway:
                                    stats.TakeAwaySales += amountBeingAdded;
                                    stats.TakeAwaySales += amountDiscounted;
                                    break;
                            }

                            stats.TotalSales += amountBeingAdded;

                            foreach (OrderSection section in orderSections)
                            {
                                foreach (SellableItem item in section.Items)
                                {
                                    if (item.ItemName == orderItem.ItemName)
                                    {
                                        stats.SectionSales[section.SectionName] += amountBeingAdded;
                                        stats.SectionSales[section.SectionName+ "Discount"] += amountDiscounted;
                                        
                                        itemLogged = true;

                                        break;
                                    }
                                }

                                if (itemLogged)
                                {
                                    break;
                                }
                            }

                            if (!itemLogged)
                            {

                                if (orderItem.ItemName.Contains("Discount"))
                                {
                                    stats.TotalDiscount += orderItem.GetTotalCost();
                                }
                                else if (orderItem.ItemName.Contains("Manual Entry"))
                                {
                                    stats.TotalManualEntry += amountBeingAdded;
                                }
                                else if (orderItem.ItemName.Contains("Refund"))
                                {
                                    stats.TotalRefund += orderItem.GetTotalCost();
                                }
                                else
                                {
                                    int currentTotal = stats.SectionSales["Unknown"];
                                    currentTotal += amountBeingAdded;
                                    stats.SectionSales["Unknown"] = currentTotal;
                                }
                            }
                        }
                    }

                    conn.Close();
                }

                return stats;
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, int> GetDailyItemBreakdown(DateTime dateTime, List<OrderSection> orderSections)
        {
            try
            {
                Dictionary<string, int> itemBreakdown = new Dictionary<string, int>();

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
                {
                    conn.Open();

                    string dateCreated = dateTime.Year.ToString("0000") + dateTime.Month.ToString("00") + dateTime.Day.ToString("00");
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT isCashSale, dataAsJson FROM DataToSync WHERE dateCreated = " + dateCreated;

                    List<DailyStatsRow> dailyStats = new List<DailyStatsRow>();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // convert query into list
                            DailyStatsRow statsRow = new DailyStatsRow();
                            statsRow.IsCashSale = reader["isCashSale"].ToString() == "1";
                            
                            byte[] data = JsonConvert.DeserializeObject<byte[]>(reader["dataAsJson"].ToString());
                            string dataAsJson = Encoding.UTF8.GetString(data);

                            CurrentOrder currentOrder = JsonConvert.DeserializeObject<CurrentOrder>(dataAsJson);
                            statsRow.Items = currentOrder.Items;
                            statsRow.OrderDiscountInPercent = currentOrder.OrderDiscountInPercent;
                            dailyStats.Add(statsRow);
                        }
                    }


                    foreach (DailyStatsRow row in dailyStats)
                    {
                        foreach (AddItemPipeline item in row.Items)
                        {
                            string itemName = item.ItemName.Replace("\n"," ");
                            if (!itemBreakdown.Keys.Contains(itemName))
                            {
                                itemBreakdown.Add(itemName, 0);
                            }

                            itemBreakdown[itemName] += ((item.GetTotalCost() * (100 - row.OrderDiscountInPercent)) / 100);
                        }
                    }


                    conn.Close();
                }

                return itemBreakdown;
            }
            catch
            {
                return null;
            }
        }

        public static DailyItemSectionBreakdownStats GetDailyItemSectionBreakdown(DateTime dateTime, string itemSection)
        {
            try
            {
                DailyItemSectionBreakdownStats stats = new DailyItemSectionBreakdownStats();

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + SqliteDbFileName + "; Version = 3; New = True; Compress = True; "))
                {
                    conn.Open();

                    string dateCreated = dateTime.Year.ToString("0000") + dateTime.Month.ToString("00") + dateTime.Day.ToString("00");
                    SQLiteCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT isCashSale, orderType, dataAsJson FROM DataToSync WHERE dateCreated = " + dateCreated;

                    List<DailyStatsRow> dailyStats = new List<DailyStatsRow>();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // convert query into list
                            DailyStatsRow statsRow = new DailyStatsRow();
                            statsRow.IsCashSale = reader["isCashSale"].ToString() == "1";
                            statsRow.OrderType = (OrderType)System.Enum.Parse(typeof(OrderType), reader["orderType"].ToString(), true);

                            byte[] data = JsonConvert.DeserializeObject<byte[]>(reader["dataAsJson"].ToString());
                            string dataAsJson = Encoding.UTF8.GetString(data);

                            CurrentOrder currentOrder = JsonConvert.DeserializeObject<CurrentOrder>(dataAsJson);
                            statsRow.Items = currentOrder.Items;
                            statsRow.OrderDiscountInPercent = currentOrder.OrderDiscountInPercent;
                            dailyStats.Add(statsRow);
                        }
                    }


                    foreach (DailyStatsRow row in dailyStats)
                    {
                        foreach (AddItemPipeline item in row.Items)
                        {
                            int itemCost = ((item.GetTotalCost() * (100 - row.OrderDiscountInPercent)) / 100);
                            int discount = item.GetTotalCost() - itemCost;

                            if (item.NavMenuFrom == itemSection)
                            {
                                stats.TotalDiscount += discount;
                                stats.TotalSales += itemCost;

                                if (row.IsCashSale)
                                {
                                    stats.TotalCashSales += itemCost;
                                }
                                else
                                {
                                    stats.TotalCardSales += itemCost;
                                }

                                switch (row.OrderType)
                                {
                                    case OrderType.OnSite:
                                        stats.OnSiteSales += itemCost;
                                        break;

                                    case OrderType.JustEat:
                                        stats.JustEatSales += itemCost;
                                        break;

                                    case OrderType.Takeaway:
                                        stats.TakeawaySales += itemCost;
                                        break;
                                }

                                string itemName = item.ItemName.Replace("\n", " ");
                                if (!stats.ItemBreakdown.Keys.Contains(itemName))
                                {
                                    stats.ItemBreakdown.Add(itemName, 0);
                                }

                                stats.ItemBreakdown[itemName] += itemCost;
                            }
                        }
                    }


                    conn.Close();
                }

                return stats;
            }
            catch
            {
                return null;
            }
        }

    }
}
