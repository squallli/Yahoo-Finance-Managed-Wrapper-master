using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace yahooFinance
{
    public class Stock
    {
        public string 股票代號 { set; get; }
        public string 時間 { set; get; }
        public string 成交 { set; get; }
        public string 昨收 { set; get; }
        public string 漲跌 { set; get; }
        public string 漲跌百分比 { set; get; }
        public string 張數 { set; get; }
        public string 開盤 { set; get; }
        public string 當日最高 { set; get; }
        public string 當日最低 { set; get; }
        public string SerialNo { set; get; }
        public stockTimeLine timeLine = null;


        public string GetSqlString()
        {
            string sql = "insert into StockPrice(StockId,Quantity,StartPrice,HPrice,LPrice,Price,Date,SerialNo) values('" + 股票代號 + "'," +
                                                  張數.Replace(",", "") + "," + 開盤.Replace(",", "") + "," + 當日最高.Replace(",", "") + "," + 當日最低.Replace(",", "") + "," + 成交.Replace(",", "") + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," + SerialNo + ")";
            return sql;
        }

      

        public bool GetStockInfo()
        {
            bool retValue = true;
            try
            {
                // 下載 Yahoo 奇摩股市資料 (範例為 2317 鴻海) 
                WebClient client = new WebClient();
                MemoryStream ms = new MemoryStream(client.DownloadData("http://tw.stock.yahoo.com/q/q?s=" + 股票代號));

                // 使用預設編碼讀入 HTML 
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(ms, Encoding.Default);
                
                // 裝載第一層查詢結果 
                HtmlAgilityPack.HtmlDocument docStockContext = new HtmlAgilityPack.HtmlDocument();

                if (doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1]") == null) return false;

                docStockContext.LoadHtml(doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1]").InnerHtml);

                // 取得個股標頭 
                HtmlNodeCollection nodeHeaders = docStockContext.DocumentNode.SelectNodes("./tr[1]/th");
                // 取得個股數值 
                string[] values = docStockContext.DocumentNode.SelectSingleNode("./tr[2]").InnerText.Trim().Split('\n');
                

                this.時間 = values[1].Trim();
                this.成交 = values[2].Trim().Replace("－","0.00");
                this.漲跌 = values[5].Trim().Replace("▽", "").Replace("▼", "").Replace("△", "").Replace("－", "0.00").Replace("▲","");
                this.張數 = values[6].Trim().Replace("－", "0.00").Replace(",","");
                this.開盤 = values[8].Trim().Replace("－", "0.00");
                this.當日最高 = values[9].Trim().Replace("－", "0.00");
                this.當日最低 = values[10].Trim().Replace("－", "0.00");
                this.昨收 = values[7].Trim().Replace("－", "0.00");
                this.漲跌百分比 = (double.Parse(成交) / double.Parse(昨收)).ToString("0.00");

                doc = null;
                docStockContext = null;
                client.Dispose();
                client = null;
                ms.Close();
            }
            catch
            {
                retValue = false; 
            }

            //timeLine = new stockTimeLine(股票代號);
            return retValue;
        }
    }

    public class stockTimeLine
    {
        public string Date = "";
        public string Time = "";
        public string Quantity = "";
        public string price = "";
        public ArrayList timeLine = new ArrayList();

        public stockTimeLine(string StockNo)
        {
            ///html[1]/body[1]/center[1]/table[3]/tbody[1]/tr[2]/td[1]/table[1]/tbody[1]/tr[2]
            try
            {
                // 下載 Yahoo 奇摩股市資料 (範例為 2317 鴻海) 
                WebClient client = new WebClient();
                MemoryStream ms = new MemoryStream(client.DownloadData("https://tw.stock.yahoo.com/q/ts?s=" + StockNo));

                // 使用預設編碼讀入 HTML 
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(ms, Encoding.Default);

                // 裝載第一層查詢結果 
                HtmlAgilityPack.HtmlDocument docStockContext = new HtmlAgilityPack.HtmlDocument();

                if (doc.DocumentNode.SelectSingleNode("/*[@id=\"yui_3_5_0_13_1438737344368_7\"]/table[3]/tbody/tr[2]") == null) return;

                docStockContext.LoadHtml(doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1]").InnerHtml);

                // 取得個股標頭 
                HtmlNodeCollection nodeHeaders = docStockContext.DocumentNode.SelectNodes("./tr[1]/th");
                // 取得個股數值 
                string[] values = docStockContext.DocumentNode.SelectSingleNode("./tr[2]").InnerText.Trim().Split('\n');

                
                doc = null;
                docStockContext = null;
                client = null;
                ms.Close();
            }
            catch
            {
              
            }
           
        }
    }

}
