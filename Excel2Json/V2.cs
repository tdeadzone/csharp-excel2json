using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.Collections;

namespace Excel2Json
{

    public class FundItem
    {
        public string id { get; set; }                      // 基金管理人银河编码
        public string name { get; set; }                    // 基金管理人
        // public string start { get; set; }                // 公募管理开始日期 
        public string end { get; set; }                     // 年末 
        public int allAmount { get; set; }                  // 基金数量（只）
        public double allWorth { get; set; }                // 资产净值（亿元）
        public int ridAmount { get; set; }                  // 剔除货币后的基金数量（只）
        public double ridWorth { get; set; }                // 剔除货币后的资产净值（亿元）
    }

    public class AmountAndWorth
    {
        public int fundAllAmount { get; set; }
        public int fundRidAmount { get; set; }
        public double fundAllWorth { get; set; }
        public double fundRidWorth { get; set; }
    }

    public class IdCollection
    {
        public Dictionary<string, Dictionary<string, AmountAndWorth>> idItems = new Dictionary<string, Dictionary<string, AmountAndWorth>>();
    }

    public class FundItemCollection
    {
        public Dictionary<string, List<FundItem>> fundItemCollection = new Dictionary<string, List<FundItem>>();
    }

    public class Read_From_Excel
    {
        public static void Main(string[] args)
        {
            string xlsPath = @"D:\csharp\data.xls";
            //string jsonPath = @"C:\Users\ruirui5\source\repos\Excel2Json\SubjectFund20\data.js";
            string jsonPath = @"D:\datatmp.json";

            Excel.Application xlsApp = new Excel.Application();
            Excel.Workbook xlsWorkbook = xlsApp.Workbooks.Open(xlsPath);
            Excel._Worksheet xlsWorksheet = xlsWorkbook.Sheets[1];
            Excel.Range xlsRange = xlsWorksheet.UsedRange;
            Excel.Range cells = xlsRange.Cells;

            int rowCount = xlsRange.Rows.Count; // 行数
            int colCount = xlsRange.Columns.Count; // 列数

            // 清理JSON
            File.WriteAllText(jsonPath, String.Empty);

            FundItemCollection collection = new FundItemCollection();

            // 根据基金管理人银河编码区别基金
            for (int row = 1; row <= rowCount; row += 1)
            {
                FundItem item = new FundItem();
                item.id = cells[row, 1].Value;
                item.name = cells[row, 2].Value;
                // item.start = Convert.ToDateTime(cells[row, 3].Value.ToString()).ToString("yyyy/MM/dd");
                item.end = Convert.ToDateTime(cells[row, 4].Value.ToString()).ToString("yyyy");
                item.allAmount = (int)cells[row, 5].Value;
                item.allWorth = cells[row, 6].Value;
                item.ridAmount = (int)cells[row, 7].Value;
                item.ridWorth = cells[row, 8].Value;
                List<FundItem> items = new List<FundItem>();
                items.Add(item);

                if (!collection.fundItemCollection.ContainsKey(item.id))
                {
                    collection.fundItemCollection.Add(item.id, items);
                }
                else
                {
                    collection.fundItemCollection[item.id].Add(item);
                }
            }

            // 生成单个基金在1998-2017这条时间线上对应的数据
            IdCollection idCollection = new IdCollection();
            string[] timeRange = { "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006",
                "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017" };
            foreach (var lists in collection.fundItemCollection)
            {
                // lists 根据基金管理人银河编码区别基金 
                var id = lists.Key;
                foreach (var item in lists.Value)
                {
                    // item 每个基金每个年份的数据
                    AmountAndWorth aw = new AmountAndWorth();
                    aw.fundAllWorth = item.allWorth;
                    aw.fundAllAmount = item.allAmount;
                    aw.fundRidWorth = item.ridWorth;
                    aw.fundRidAmount = item.ridAmount;
                    Dictionary<string, AmountAndWorth> dict = new Dictionary<string, AmountAndWorth>();
                    dict.Add(item.end, aw);
                    if (!idCollection.idItems.ContainsKey(id))
                    {
                        idCollection.idItems.Add(id, dict);
                    }
                    else
                    {
                        idCollection.idItems[id].Add(item.end, aw);
                    }
                }
            }

            // Console.WriteLine(collection.fundItemCollection.Count);

            string output = JsonConvert.SerializeObject(idCollection);
            //output = "window.__data__ = " + output;
            using (StreamWriter file =
                            new StreamWriter(jsonPath, true))
            {
                file.WriteLine(output);
            }

            // 清理
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // 发布
            Marshal.ReleaseComObject(xlsRange);
            Marshal.ReleaseComObject(xlsWorksheet);

            xlsWorkbook.Close();
            Marshal.ReleaseComObject(xlsWorkbook);

            xlsApp.Quit();
            Marshal.ReleaseComObject(xlsApp);
        }
    }
}