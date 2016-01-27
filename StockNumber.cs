using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace yahooFinance
{
    public class StockNo
    {
        private ArrayList stockArr = new ArrayList();
        public string StockNumber
        {
            set;
            get;
        }

        public string StockName
        {
            set;
            get;
        }

        public string Status
        {
            set;
            get;
        }

        public string StockType
        {
            set;
            get;
        }


        public ArrayList GetAllStock()
        {
            // 下載台灣上市、上櫃公司代號 
           
            MemoryStream ms = getMemoryStreamByUrl("http://isin.twse.com.tw/isin/C_public.jsp?strMode=2");

            // 使用預設編碼讀入 HTML 
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(ms, Encoding.Default);

            // 裝載第一層查詢結果 
            HtmlAgilityPack.HtmlDocument docStockContext = new HtmlAgilityPack.HtmlDocument();
            //HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[contains(@bgcolor,'#FAFAD2')]"); //上市
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table")[1].SelectNodes("tr");

            int ret = 0;
            string str = "";

            ArrayList stockNo = new ArrayList();
            StockNo s = null;
            for (int i = 2; i < nodes.Count; i++)
            {
                HtmlNodeCollection tds = nodes[i].SelectNodes("td");
                if (tds[0].InnerText.Replace(" ", "").Replace("　", "") == "上市認購(售)權證") break;
                foreach (HtmlNode td in tds)
                {
                    str = td.InnerText.Replace(" ", "").Replace("　", "");
                    if (str == "") continue;
                    if(str.IndexOf("/") >=0 ) continue;
                    if (str == "上市認購(售)權證") break;
                    if (!int.TryParse(str[0].ToString(), out ret)) continue;
                    if (stockNo.Contains(str.Substring(0, 4))) continue;
                    else stockNo.Add(str.Substring(0, 4));

                    s = new StockNo();
                    s.StockNumber = str.Substring(0, 4);
                    s.StockName = str.Substring(4);
                    s.Status = "A";
                    s.StockType = "1";
                    if (!stockArr.Contains(s))
                        stockArr.Add(s);
                }
            }
            ms.Close();
            ms.Dispose();

            ms = getMemoryStreamByUrl("http://isin.twse.com.tw/isin/C_public.jsp?strMode=4");
            doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(ms, Encoding.Default);

            // 裝載第一層查詢結果 
            docStockContext = new HtmlAgilityPack.HtmlDocument();
            //nodes = doc.DocumentNode.SelectNodes("//*[contains(@bgcolor,'#FAFAD2')]"); //上櫃
            nodes = doc.DocumentNode.SelectNodes("//table")[1].SelectNodes("tr");
            bool isFind = false;

            for (int i = 2; i < nodes.Count; i++)
            {
                HtmlNodeCollection tds = nodes[i].SelectNodes("td");
                if (tds.Count == 1 && tds[0].InnerText.Trim() == "股票")
                {
                    isFind = true;
                    continue;
                }
                else if(tds.Count == 1 && tds[0].InnerText.Trim() != "股票") isFind = false;

                if (!isFind) continue;

                foreach (HtmlNode td in tds)
                {
                    str = td.InnerText.Replace(" ", "").Replace("　", "");
                    if (str == "") continue;
                    if (!int.TryParse(str[0].ToString(), out ret)) continue;
                    if (stockNo.Contains(str.Substring(0, 4))) continue;
                    else stockNo.Add(str.Substring(0, 4));

                    s = new StockNo();
                    s.StockNumber = str.Substring(0, 4);
                    s.StockName = str.Substring(4);
                    s.Status = "A";
                    s.StockType = "2";
                    if (!stockArr.Contains(s))
                        stockArr.Add(s);
                }
            }
            ms.Close();
            ms.Dispose();
            //foreach (HtmlNode n in nodes)
            //{
            //    s = new StockNo();
            //    str = n.InnerText.Replace(" ", "").Replace("　", "");
            //    if (str == "") continue;
            //    if (str == "臺灣存託憑證") break;
            //    if (str == "股票")
            //        isFind = true;
            //    if (!int.TryParse(str[0].ToString(), out ret)) continue;

                

            //    if (!isFind) continue;
                
            //    if (stockNo.Contains(str.Substring(0, 4))) continue;
            //    else stockNo.Add(str.Substring(0, 4));

            //    s.StockNumber = str.Substring(0, 4);
            //    s.StockName = str.Substring(4);
            //    s.Status = "A";
            //    s.StockType = "2";
            //    if (!stockArr.Contains(s))
            //        stockArr.Add(s);
            //}

            //ms.Close();
            //ms.Dispose();
            return stockArr;
        }

        private MemoryStream getMemoryStreamByUrl(string Url)
        {
            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData(Url));

            return ms;
        }
    }
}
