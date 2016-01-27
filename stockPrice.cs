using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Collections;
using System.Data.SQLite;
using System.Configuration;

namespace yahooFinance
{
    public class clsStockInfo
    {
        public string 股票代號 { set; get; }
        public string 成交 { set; get; }
        public string 張數 { set; get; }
        public string 開盤 { set; get; }
        public string 當日最高 { set; get; }
        public string 當日最低 { set; get; }
        public string 日期 { set; get; }
    }

    public class stockPrice
    {
        ArrayList stockArray = new ArrayList();
        public string 股票代號 { set; get; }
        public stockPrice(string StockId)
        {
             
            this.股票代號 = StockId;
        }


        public void GetStockInfo()
        {
            try
            {
                DateTime now = DateTime.Parse("2015/07/01");
                string year = now.ToString("yyyy");
                string Month = now.ToString("MM");
                // 下載 Yahoo 奇摩股市資料 (範例為 2317 鴻海) 
                WebClient client = new WebClient();
                MemoryStream ms = new MemoryStream(client.DownloadData("http://www.twse.com.tw/ch/trading/exchange/STOCK_DAY/genpage/Report" + year + Month + "/" + year +Month + "_F3_1_8_" + 股票代號 + ".php?STK_NO=" + 股票代號 + "&myear=" + year + "&mmon=" + Month));

                // 使用預設編碼讀入 HTML 
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(ms, Encoding.Default);
                
                // 裝載第一層查詢結果 
                HtmlAgilityPack.HtmlDocument docStockContext = new HtmlAgilityPack.HtmlDocument();

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[contains(@class,'board_trad')]")[0].SelectNodes("tr");

                clsStockInfo s = null;

                for (int i = 2; i < nodes.Count; i++)
                {
                    s = new clsStockInfo();

                    HtmlNodeCollection tds = nodes[i].SelectNodes("td");
                    s.股票代號 = 股票代號;
                    s.日期 = tds[0].InnerText.Trim();
                    s.張數 = tds[1].InnerText.Trim();

                    if (tds[3].InnerText.Trim() == "--") s.開盤 = "0";
                    else s.開盤 = tds[3].InnerText.Trim();


                    if (tds[5].InnerText.Trim() == "--") s.當日最低 = "0";
                    else s.當日最低 = tds[5].InnerText.Trim();

                    if (tds[4].InnerText.Trim() == "--") s.當日最高 = "0";
                    else s.當日最高 = tds[4].InnerText.Trim();

                    if (tds[6].InnerText.Trim() == "--") s.成交 = "0";
                    else s.成交 = tds[6].InnerText.Trim();

                    stockArray.Add(s);
                    
                }
               
                

                doc = null;
                docStockContext = null;
                client.Dispose();
                client = null;
                ms.Close();



                using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
                {
                    conn.Open();
                    foreach (clsStockInfo info in stockArray)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            try
                            {
                                cmd.CommandText = "insert into StockPrice(StockId,Quantity,StartPrice,HPrice,LPrice,Price,Date) values('" + info.股票代號 + "'," +
                                                  info.張數.Replace(",", "") + "," + info.開盤.Replace(",", "") + "," + info.當日最高.Replace(",", "") + "," + info.當日最低.Replace(",", "") + "," + info.成交.Replace(",", "") + ",'" + TransferDate(info.日期) + "')";

                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            catch
            {
               
            }
        }

        public void GetStockInfo上櫃()
        {
            DateTime now = DateTime.Parse("2015/07/01");
            string year = (now.Year - 1911).ToString();
            string Month = now.ToString("MM");

            try
            {
                // 下載 Yahoo 奇摩股市資料 (範例為 2317 鴻海) 
                WebClient client = new WebClient();
                MemoryStream ms = new MemoryStream(client.DownloadData("http://www.tpex.org.tw/web/stock/aftertrading/daily_trading_info/st43_print.php?l=zh-tw&d=" + year + "/" + Month + "&stkno=" + 股票代號 + "&s=0,asc,0"));

                // 使用預設編碼讀入 HTML 
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(ms, Encoding.Default);

                // 裝載第一層查詢結果 
                HtmlAgilityPack.HtmlDocument docStockContext = new HtmlAgilityPack.HtmlDocument();

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table/tbody/tr");

                clsStockInfo s = null;

                for (int i = 2; i < nodes.Count; i++)
                {
                    s = new clsStockInfo();

                    HtmlNodeCollection tds = nodes[i].SelectNodes("td");
                    s.股票代號 = 股票代號;
                    s.日期 = tds[0].InnerText.Trim();
                    s.張數 = tds[1].InnerText.Trim();

                    if (tds[3].InnerText.Trim() == "--") s.開盤 = "0";
                    else s.開盤 = tds[3].InnerText.Trim();


                    if (tds[5].InnerText.Trim() == "--") s.當日最低 = "0";
                    else s.當日最低 = tds[5].InnerText.Trim();

                    if (tds[4].InnerText.Trim() == "--") s.當日最高 = "0";
                    else s.當日最高 = tds[4].InnerText.Trim();

                    if (tds[6].InnerText.Trim() == "--") s.成交 = "0";
                    else s.成交 = tds[6].InnerText.Trim();

                    stockArray.Add(s);

                }



                doc = null;
                docStockContext = null;
                client.Dispose();
                client = null;
                ms.Close();



                using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
                {
                    conn.Open();
                    foreach (clsStockInfo info in stockArray)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            try
                            {
                                cmd.CommandText = "insert into StockPrice(StockId,Quantity,StartPrice,HPrice,LPrice,Price,Date) values('" + info.股票代號 + "'," +
                                                  info.張數.Replace(",", "") + "," + info.開盤.Replace(",", "") + "," + info.當日最高.Replace(",", "") + "," + info.當日最低.Replace(",", "") + "," + info.成交.Replace(",", "") + ",'" + TransferDate(info.日期) + "')";

                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private string TransferDate(string 民國年)
        {
            try
            {
                string[] str = 民國年.Split('/');
                string Date = (int.Parse(str[0]) + 1911).ToString() + "-" + str[1] + "-" + str[2];

                return Date;
            }
            catch
            {
                return "1911/01/01";
            }
            
        }
    }
    
}
