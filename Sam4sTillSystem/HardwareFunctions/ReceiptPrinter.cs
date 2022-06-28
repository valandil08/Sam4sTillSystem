using Sam4sTillSystem.Data;
using Sam4sTillSystem.Data.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Management;
using System.Threading;
using System.Linq;
using Sam4sTillSystem.Data.JsonObject.MenuSetting;
using Sam4sTillSystem.Sqlite.Model;

namespace Sam4sTillSystem.Helpers
{    

    public static class ReceiptPrinter
    {
        public class PrinterQueue<T>
        {
            private List<T> Queue = new List<T>();
            private Thread PrinterThread = null;
            private Func<T, bool> PrintDocumentCode = null;

            public PrinterQueue(Func<T, bool> printDocumentCode)
            {
                PrintDocumentCode = printDocumentCode;
            }

            public bool QueueDocument(T document)
            {
                try
                {
                    EnsurePrinterThreadInitilized();

                    Queue.Add(document);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private void EnsurePrinterThreadInitilized()
            {
                if (PrinterThread == null)
                {
                    PrinterThread = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;

                        while (true)
                        {
                            if (Queue.Count > 0)
                            {
                                bool printed = PrintDocumentCode.Invoke(Queue[0]);

                                if (printed)
                                {
                                    Queue.RemoveAt(0);
                                }
                                else
                                {
                                    QueueDocument(Queue[0]);
                                    Queue.RemoveAt(0);
                                }

                            }
                            Thread.Sleep(100);
                        }
                    });
                    PrinterThread.Start();
                }
            }
        }

        public static PrinterQueue<OrderInfo> CustomerReceiptQueue = new PrinterQueue<OrderInfo>(PrintCustomerReceipt);
        public static PrinterQueue<OrderInfo> ChefsNoteQueue = new PrinterQueue<OrderInfo>(PrintChefsNotes);
        public static PrinterQueue<DateTime> DailyStatsQueue = new PrinterQueue<DateTime>(PrintDailyStats);

        private static bool PrintCustomerReceipt(OrderInfo orderInfo)
        {
            PrintPageEventHandler handler = new PrintPageEventHandler
              (
                  (object sender, PrintPageEventArgs e) => {

                      DateTime dateTime = DateTime.Now;
                      Graphics graphics = e.Graphics;
                      Font font = new Font("Courier New", 13);
                      Font boldFont = new Font("Courier New", 13, FontStyle.Bold);
                      Font titleFont = new Font("Courier New", 20, FontStyle.Bold);
                      SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);
                      const string blankLine = "------------------------";

                      float fontHeight = font.GetHeight();

                      int xPos = 0;
                      int yPos = 0;
                      int offset = 20;

                      graphics.DrawString("   Receipt", titleFont, brush, xPos + 10, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;
                      graphics.DrawString(CreateTextRow("   Order Information", ""), boldFont, brush, xPos, yPos);
                      yPos += offset;
                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;

                      bool firstItem = true;
                      foreach (OrderItemInfo orderItemInfo in orderInfo.OrderItems)
                      {
                          int totalItemCost = orderItemInfo.GetTotalCostEachInPence();

                          if (orderItemInfo.ProductButtonId < 0)
                          {
                              if (firstItem)
                              {
                                  firstItem = false;
                              }
                              else
                              {
                                  yPos += offset;
                                  yPos += offset;
                              }

                              switch (orderItemInfo.ProductButtonId)
                              {
                                  case -1:
                                      graphics.DrawString(CreateTextRow("Manual Entry", UIHelper.FormatPrice(totalItemCost)), boldFont, brush, xPos, yPos);
                                      break;

                                  case -2:
                                      graphics.DrawString(CreateTextRow("Fixed Discount", UIHelper.FormatPrice(totalItemCost)), boldFont, brush, xPos, yPos);
                                      break;

                                  case -3:
                                      graphics.DrawString(CreateTextRow("Refund", UIHelper.FormatPrice(totalItemCost)), boldFont, brush, xPos, yPos);
                                      break;
                              }

                              continue;
                          }

                          ProductButton button = MenuHelper.GetProductButton(orderItemInfo.ProductButtonId);

                          for (int i = 0; i < orderItemInfo.Quantity; i++)
                          {
                              if (firstItem)
                              {
                                  firstItem = false;
                              }
                              else
                              {
                                  yPos += offset;
                                  yPos += offset;
                              }
                              graphics.DrawString(CreateTextRow(button.ReceiptText, UIHelper.FormatPrice(totalItemCost)), boldFont, brush, xPos, yPos);
                             


                              List<string> textToRender = new List<string>();

                              for (int screenNumber = 0; screenNumber < orderItemInfo.ScreenValues.Count; screenNumber++)
                              {
                                  int[] controlValues = new int[0];

                                  if (orderItemInfo.ScreenValues.ContainsKey(screenNumber))
                                  {
                                      controlValues = orderItemInfo.ScreenValues[screenNumber];
                                  }

                                  bool isSwapIns = screenNumber == orderItemInfo.ProductScreenIds.Length;
                                  bool isSwapOuts = screenNumber == orderItemInfo.ProductScreenIds.Length + 1;

                                  for (int j = 0; j < controlValues.Length; j++)
                                  {
                                      int controlId = 0;

                                      if (isSwapIns)
                                      {
                                          controlId = GlobalData.MenuSettings.GenericExtraScreenControls[j];
                                      }
                                      else if (isSwapOuts)
                                      {
                                          controlId = GlobalData.MenuSettings.ProductButtons.Where(x => x.ProductButtonId == orderItemInfo.ProductButtonId).First().AdjustmentControlIds[j];
                                      }
                                      else
                                      {
                                          int screenId = orderItemInfo.ProductScreenIds[screenNumber];
                                          controlId = GlobalData.MenuSettings.ProductScreens.Where(x => x.ProductScreenId == screenId).First().ProductScreenControlIds[j];
                                      }

                                      ProductScreenControl control = GlobalData.MenuSettings.ProductScreenControls.Where(x => x.ProductScreenControlId == controlId).First();

                                      int difference;

                                      if (control.IsQuantitySetter)
                                      {
                                          difference = controlValues[j] - control.StartingQuantity;
                                      }
                                      else
                                      {
                                          difference = controlValues[j] - control.StartingQuantity;
                                      }

                                      if (difference == 0)
                                      {
                                          continue;
                                      }



                                      if (isSwapOuts)
                                      {
                                          if (controlValues[j] == 0)
                                          {
                                              textToRender.Add("- No " + control.ButtonText);
                                          }
                                          else
                                          {
                                              textToRender.Add("- Only " + controlValues[j] + " " + control.ButtonText);
                                          }
                                      }
                                      else if (isSwapIns)
                                      {
                                          textToRender.Add(CreateTextRow("+ " + control.ButtonText, "x"+controlValues[j]));
                                      }
                                      else
                                      {
                                          if (control.IsQuantitySetter)
                                          {
                                              textToRender.Add(CreateTextRow("+ " + control.ButtonText, "x" + controlValues[j]));
                                          }
                                          else
                                          {
                                              textToRender.Add("* " + control.ButtonText);
                                          }

                                      }
                                  }

                              }

                              textToRender = textToRender.OrderBy(x => x.StartsWith("No") ? "z" + x.Substring(3) : x.StartsWith("Only") ? "y" + x.Substring(5) : x.StartsWith("*") ? "a" + x.Substring(2) : "x" + x.Substring(3)).ToList();

                              if (textToRender.Count > 0)
                              {
                                  yPos += offset;
                              }

                              bool firstTextToRender = true;
                              foreach (string text in textToRender)
                              {
                                  if (firstTextToRender)
                                  {
                                      firstTextToRender = false;
                                  }
                                  else
                                  {
                                      yPos += 20;
                                  }
                                  graphics.DrawString(text, font, brush, xPos, yPos);
                              }

                          }
                       
                      }

                      yPos += offset;
                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += 10;
                      if (orderInfo.PercentageBasedOrderDiscount != 0)
                      {
                          double vat = orderInfo.Vat;

                          orderInfo.Vat = 0;
                          graphics.DrawString(CreateTextRow("Original Total: ", UIHelper.FormatPrice(orderInfo.GetOriginalTotalCostInPence())), font, brush, xPos, yPos);
                          orderInfo.Vat = vat;
                          yPos += offset;
                      }

                      if (orderInfo.PercentageBasedOrderDiscount != 0)
                      {
                          graphics.DrawString(CreateTextRow("Order Discount: ", orderInfo.PercentageBasedOrderDiscount + "%"), font, brush, xPos, yPos);
                          yPos += offset;
                      }
                      graphics.DrawString(CreateTextRow("Total: ", UIHelper.FormatPrice(orderInfo.GetTotalCostInPence())), font, brush, xPos, yPos);
                      yPos += offset;
                      graphics.DrawString(CreateTextRow("VAT Included: ", UIHelper.FormatPrice(orderInfo.GetTotalVatInPence())), font, brush, xPos, yPos);
                      yPos += offset;
                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;
                      graphics.DrawString(CreateTextRow("   Other Information", ""), boldFont, brush, xPos, yPos + 5);
                      yPos += offset;
                      yPos += 10;
                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;
                      graphics.DrawString(CreateTextRow("Server", GlobalData.UserLogin.Name), font, brush, xPos, yPos);
                      yPos += offset;
                      graphics.DrawString(CreateTextRow("Date", dateTime.Day.ToString("00") + "/" + dateTime.Month.ToString("00") + "/" + dateTime.Year.ToString("0000")), font, brush, xPos, yPos);
                      yPos += offset;
                      graphics.DrawString(CreateTextRow("Time", dateTime.Hour.ToString("00") + ":" + dateTime.Minute.ToString("00") + ":" + dateTime.Second.ToString("00")), font, brush, xPos, yPos);
                      if (orderInfo.VatNumber != null)
                      {
                      yPos += offset;
                      graphics.DrawString(CreateTextRow("VAT Number: ", orderInfo.VatNumber), font, brush, xPos, yPos);

                      }
                      yPos += offset;
                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                  }
              );

            bool printed = PrintDocWithPrinter(GlobalData.HardwareSettings.ReceiptPrinterName, handler);

            if (!printed)
            {
                printed = PrintDocWithPrinter(GlobalData.HardwareSettings.BackupReceiptPrinterOneName, handler);
            }

            if (!printed)
            {
                printed = PrintDocWithPrinter(GlobalData.HardwareSettings.BackupReceiptPrinterTwoName, handler);
            }

            return printed;
        }

        private static bool PrintChefsNotes(OrderInfo orderInfo)
        {
            if (!orderInfo.OrderItems.Where(x =>( x.ProductButtonId < -1 ) ? false : (x.ProductButtonId == -1 || MenuHelper.GetProductButton(x.ProductButtonId).IncludeInChefsNotes)).Any())
            {
                return true;
            }

            PrintPageEventHandler handler = new PrintPageEventHandler
              (
                  (object sender, PrintPageEventArgs e) => {
                      DateTime dateTime = DateTime.Now;
                      Graphics graphics = e.Graphics;
                      Font normalFont = new Font("Courier New", 13);
                      Font italicFont = new Font("Courier New", 13, FontStyle.Italic);
                      Font boldFont = new Font("Courier New", 13, FontStyle.Bold);
                      SolidBrush brush = new SolidBrush(Color.Black);

                      int xPos = 0;
                      int yPos = 0;

                      yPos += 120;
                      graphics.DrawString("Server: " + GlobalData.UserLogin.Name, boldFont, brush, xPos, yPos);
                      yPos += 20;
                      yPos += 20;
                      DateTime time = DateTime.Now;
                      graphics.DrawString("Time: " + time.Hour.ToString("00") + ":" + time.Minute.ToString("00") + ":" + time.Second.ToString("00"), boldFont, brush, xPos, yPos);
                      yPos += 20;
                      yPos += 20;
                      switch (orderInfo.DestinationNumber)
                      {
                          case 88888:
                              graphics.DrawString("Table Number: Just Eat", boldFont, brush, xPos, yPos);
                              break;

                          case 99999:
                              graphics.DrawString("Table Number: Takeaway", boldFont, brush, xPos, yPos);
                              break;

                          default:
                              graphics.DrawString("Table Number: " + orderInfo.DestinationNumber, boldFont, brush, xPos, yPos);
                              break;
                      }
                      yPos += 20;
                      yPos += 20;
                      foreach (OrderItemInfo orderItemInfo in orderInfo.OrderItems)
                      {
                          if (orderItemInfo.ProductButtonId < 0)
                          {
                              switch(orderItemInfo.ProductButtonId)
                              {
                                  case -1: // Manual Entry
                                      graphics.DrawString("Manual Entry", boldFont, brush, xPos, yPos);
                                      yPos += 40;
                                      break;
                              }
                              continue;
                          }

                          ProductButton button = MenuHelper.GetProductButton(orderItemInfo.ProductButtonId);

                          if (button.IncludeInChefsNotes == false)
                          {
                              continue;
                          }

                          for (int x = 0; x < orderItemInfo.Quantity; x++)
                          {

                              graphics.DrawString(button.ChefNoteText, boldFont, brush, xPos, yPos);
                              yPos += 20;

                              for (int i = 0; i < orderItemInfo.ProductScreenIds.Count(); i++)
                              {
                                  ProductScreen screen = MenuHelper.GetProductScreen(orderItemInfo.ProductScreenIds[i]);

                                  int[] controlValues = orderItemInfo.ScreenValues[i];

                                  for (int j = 0; j < controlValues.Length; j++)
                                  {
                                      if (controlValues[j] == 0)
                                      {
                                          continue;
                                      }

                                      ProductScreenControl control = MenuHelper.GetProductScreenControl(screen.ProductScreenControlIds[j]);

                                      string prefix = "* ";

                                      if (control.IsQuantitySetter)
                                      {
                                          prefix = controlValues[j] < 0 ? controlValues[j].ToString() : "+" + controlValues[j];
                                      }

                                      graphics.DrawString(prefix + " " + control.ChefNoteText, normalFont, brush, xPos, yPos); 
                                      yPos += 20;
                                  }
                              }
                              try
                              {
                                  if (orderItemInfo.ProductScreenIds.Length > 0)
                                  {

                                      int[] adjustmentControlIds = button.AdjustmentControlIds;

                                      int[] adjustmentControlValues = orderItemInfo.ScreenValues[orderItemInfo.ScreenValues.Count - 1];

                                      for (int j = 0; j < adjustmentControlValues.Length; j++)
                                      {
                                          ProductScreenControl control = MenuHelper.GetProductScreenControl(adjustmentControlIds[j]);

                                          if (control.StartingQuantity == adjustmentControlValues[j])
                                          {
                                              continue;
                                          }

                                          if (adjustmentControlValues[j] == 0)
                                          {
                                              graphics.DrawString("No " + control.ChefNoteText, normalFont, brush, xPos, yPos);
                                          }
                                          else
                                          {
                                              graphics.DrawString("Only " + adjustmentControlValues[j] + " " + control.ChefNoteText, normalFont, brush, xPos, yPos);
                                          }

                                          yPos += 20;
                                      }

                                      int[] additonControlIds = MenuHelper.GetAdditions();

                                      int[] additionControlValues = orderItemInfo.ScreenValues[orderItemInfo.ScreenValues.Count - 2];

                                      for (int j = 0; j < additionControlValues.Length; j++)
                                      {
                                          if (additionControlValues[j] == 0)
                                          {
                                              continue;
                                          }

                                          ProductScreenControl control = MenuHelper.GetProductScreenControl(additonControlIds[j]);

                                          graphics.DrawString("+" + additionControlValues[j].ToString() + " " + control.ChefNoteText, normalFont, brush, xPos, yPos);
                                          yPos += 20;
                                      }
                                  }
                              }
                              catch
                              {

                              }
                              yPos += 40;
                          }
                      }
                      yPos += 20;
                      yPos += 20;
                      yPos += 20;
                      graphics.DrawString("", normalFont, brush, xPos, yPos);

                  }
              );

            bool printed = PrintDocWithPrinter(GlobalData.HardwareSettings.ChefsPrinterMainName, handler);

            if (!printed)
            {
                printed = PrintDocWithPrinter(GlobalData.HardwareSettings.BackupChefsPrinterOneName, handler);
            }

            if (!printed)
            {
                printed = PrintDocWithPrinter(GlobalData.HardwareSettings.BackupChefsPrinterTwoName, handler);
            }

            return printed;
        }

        private static bool PrintDailyStats(DateTime statsDay)
        {
            PrintPageEventHandler handler = new PrintPageEventHandler
              (
                  (object sender, PrintPageEventArgs e) => {

                      DailyStats dailyStats = Sqlite.SqliteData.GetDailyStats(statsDay);
                      List<int> dateTimeValues = Sqlite.SqliteData.GetListOfNoSaleActivations(statsDay);

                      int openingTime = 9;// DataHelper.GetOpeningTime(DateTime.Now, null);
                      int closingTime = 5;// DataHelper.GetClosingTime(DateTime.Now, null);

                      int timesOpenedBeforeOpeningTime = dateTimeValues.Where(x => x < openingTime).Count();
                      int timesOpenedAfterClosingTime = dateTimeValues.Where(x => x > closingTime).Count();

                      DateTime dateTime = DateTime.Now;
                      Graphics graphics = e.Graphics;
                      Font font = new Font("Courier New", 13);
                      Font titleFont = new Font("Courier New", 20);
                      SolidBrush brush = new SolidBrush(Color.Black);
                      const string blankLine = "------------------------";

                      float fontHeight = font.GetHeight();

                      int xPos = 0;
                      int yPos = 0;
                      int offset = 15;

                      graphics.DrawString(" Hilltop Cafe", titleFont, brush, xPos + 10, yPos);
                      yPos += 20;
                      yPos += 20;
                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += 20;
                      graphics.DrawString(CreateTextRow("Daily Stats", ""), font, brush, xPos + 5, yPos);
                      yPos += 20;
                      graphics.DrawString(CreateTextRow(statsDay.Date.ToShortDateString(), ""), font, brush, xPos + 5, yPos);
                      yPos += 20;
                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += 20;


                      graphics.DrawString("On Site Sales", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.OnSiteSales), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;


                      graphics.DrawString("On Site Sales Discount", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.OnSiteSalesDiscount), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Just Eat Sales", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.JustEatSales), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Just Eat Sales Discount", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.JustEatSalesDiscount), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Takeaway Sales", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TakeAwaySales), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Takeaway Sales Discount", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TakeAwaySalesDiscount), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;


                      graphics.DrawString("Total Discount Value", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalDiscount), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Total Manual Entry Value", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalManualEntry), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Total Refund Value", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalRefund), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;


                      graphics.DrawString("No Sales", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(dateTimeValues.Count.ToString(), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Total Card Sales", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalCardSales), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Total Card Sales Discount", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalCardSalesDiscount), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Total Cash Sales", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalCashSales), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Total Cash Sales Discount", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalCashSalesDiscount), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Total Sales", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalSales), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString("Total Sales Discount", font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;
                      graphics.DrawString(UIHelper.FormatPrice(dailyStats.TotalSalesDiscount), font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString(blankLine, font, brush, xPos, yPos);
                      yPos += offset;
                      yPos += offset;

                      graphics.DrawString(" ", font, brush, xPos, yPos);

                  }
              );

            bool printed = PrintDocWithPrinter(GlobalData.HardwareSettings.ReceiptPrinterName, handler);

            if (!printed)
            {
                printed = PrintDocWithPrinter(GlobalData.HardwareSettings.BackupReceiptPrinterOneName, handler);
            }

            if (!printed)
            {
                printed = PrintDocWithPrinter(GlobalData.HardwareSettings.BackupReceiptPrinterTwoName, handler);
            }

            return printed;
        }

        #region Hardware Layer Code

        private static bool PrintDocWithPrinter(string name, PrintPageEventHandler handler)
        {
            if (CheckIfPrinterOnline(name))
            {               
                try
                {
                    PrintDocument doc = new PrintDocument();
                    doc.PrinterSettings.PrinterName = name;
                    doc.PrintPage += handler;
                    doc.DefaultPageSettings.PaperSize = new PaperSize("custom", 314, 12800);
                    doc.Print();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool CheckIfPrinterOnline(string name)
        {
            if (name == "None")
            {
                return false;
            }

            ManagementScope scope = new ManagementScope(@"\root\cimv2");
            scope.Connect();

            // Select Printers from WMI Object Collections
            ManagementObjectSearcher searcher = new
            ManagementObjectSearcher("SELECT * FROM Win32_Printer WHERE Name LIKE '%" + name + "'");

            foreach (ManagementObject printer in searcher.Get())
            {
                if (printer["Name"].ToString().ToLower().Equals(name.ToLower()))
                {
                    return !printer["WorkOffline"].ToString().ToLower().Equals("true");
                }
            }

            return false;
        }

#endregion
        /*
        public static bool PrintItemSectionBreakdownItems(string sectionName, DailyItemSectionBreakdownStats stats)
        {
            try
            {
                PrintDocument doc = new PrintDocument();
                doc.PrinterSettings.PrinterName = DataHelper.GetHardwareSettings().ReceiptPrinterName;
                doc.PrintPage += new PrintPageEventHandler
                (
                    (object sender, PrintPageEventArgs e) =>  {
                        DateTime dateTime = DateTime.Now;
                        Graphics graphics = e.Graphics;
                        Font font = new Font("Courier New", 13);
                        Font titleFont = new Font("Courier New", 20);
                        SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);
                        const string blankLine = "------------------------";

                        float fontHeight = font.GetHeight();

                        int xPos = 0;
                        int yPos = 0;
                        int offset = 15;

                        e.PageSettings.PaperSize.Width = 50;
                        graphics.DrawString(" Hilltop Cafe", titleFont, brush, xPos + 10, yPos);
                        yPos += 20;
                        yPos += 20;
                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += 20;
                        graphics.DrawString(CreateTextRow(sectionName.Replace("\n", " "), ""), font, brush, xPos + 5, yPos);
                        yPos += 20;
                        graphics.DrawString(CreateTextRow(DateTime.Now.Date.ToShortDateString(), ""), font, brush, xPos + 5, yPos);
                        yPos += 20;
                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += 20;

                        foreach (string key in stats.ItemBreakdown.Keys)
                        {
                            graphics.DrawString(key.Replace("\n", " "), font, brush, xPos, yPos);
                            yPos += offset;
                            yPos += offset;
                            graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.ItemBreakdown[key]), font, brush, xPos, yPos);
                            yPos += offset;
                            yPos += offset;

                            graphics.DrawString(key.Replace("\n", " ") + " Discount", font, brush, xPos, yPos);
                            yPos += offset;
                            yPos += offset;
                            if (stats.ItemBreakdown.ContainsKey(key + "Discount"))
                            {
                                graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.ItemBreakdown[key + "Discount"]), font, brush, xPos, yPos);
                            }
                            else
                            {
                                graphics.DrawString(DataHelper.UIHelper.FormatPrice(0), font, brush, xPos, yPos);
                            }    
                            yPos += offset;
                            yPos += offset;
                        }

                        graphics.DrawString(" ", font, brush, xPos, yPos);
                        yPos += 20;
                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(" ", font, brush, xPos, yPos);
                    }
                );
                doc.Print();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool PrintItemSectionBreakdownSummary(string sectionName, DailyItemSectionBreakdownStats stats)
        {
            try
            {
                PrintDocument doc = new PrintDocument();
                doc.PrinterSettings.PrinterName = DataHelper.GetHardwareSettings().ReceiptPrinterName;
                doc.PrintPage += new PrintPageEventHandler
                (
                    (object sender, PrintPageEventArgs e) => {
                        DateTime dateTime = DateTime.Now;
                        Graphics graphics = e.Graphics;
                        Font font = new Font("Courier New", 13);
                        Font titleFont = new Font("Courier New", 20);
                        SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);
                        const string blankLine = "------------------------";

                        float fontHeight = font.GetHeight();

                        int xPos = 0;
                        int yPos = 0;
                        int offset = 15;

                        e.PageSettings.PaperSize.Width = 50;
                        graphics.DrawString(" Hilltop Cafe", titleFont, brush, xPos + 10, yPos);
                        yPos += 20;
                        yPos += 20;
                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += 20;
                        graphics.DrawString(CreateTextRow(sectionName.Replace("\n", " "), ""), font, brush, xPos + 5, yPos);
                        yPos += 20;
                        graphics.DrawString(CreateTextRow(DateTime.Now.Date.ToShortDateString(), ""), font, brush, xPos + 5, yPos);
                        yPos += 20;
                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += 20;

                        graphics.DrawString("On Site Sales", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.OnSiteSales), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("On Site Sales Discount", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.OnSiteSalesDiscount), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Takeaway Sales", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TakeawaySales), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Takeaway Sales Discount", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TakeawaySalesDiscount), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Just Eat Sales", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.JustEatSales), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Just Eat Sales Discount", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.JustEatSalesDiscount), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += 20;

                        graphics.DrawString("Total Discount", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalDiscount), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        
                        graphics.DrawString("Total Manual Entry", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalManualEntry), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Total Refund", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalRefund), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        
                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += 20;

                        graphics.DrawString("Total Card Sales", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalCardSales), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Total Card Sales Discount", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalCardSalesDiscount), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Total Cash Sales", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalCashSales), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Total Cash Sales Discount", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalCashSalesDiscount), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Total Sales", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalSales), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString("Total Sales Discount", font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(DataHelper.UIHelper.FormatPrice(stats.TotalSalesDiscount), font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;

                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += 20;

                        graphics.DrawString(" ", font, brush, xPos, yPos);
                        yPos += 20;
                        graphics.DrawString(blankLine, font, brush, xPos, yPos);
                        yPos += offset;
                        yPos += offset;
                        graphics.DrawString(" ", font, brush, xPos, yPos);
                    }
                );
                doc.Print();

                return true;
            }
            catch
            {
                return false;
            }
        }
        */
        private static string CreateTextRow(string leftText, string rightText)
        {
            string blanks = "";

            for (int i = 0; i < (24 - (leftText.Length + rightText.Length)); i++)
            {
                blanks += " ";
            }

            return leftText + blanks + rightText;
        }

    }
}
