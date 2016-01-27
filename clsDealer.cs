using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections;
using System.Net;
using System.IO;
using System.Data.SQLite;
using System.Configuration;

namespace yahooFinance
{
    public class clsDealerObj
    {
        public string StockId { set; get; }
        public string SerialNo { set; get; }
        public string DealerName { set; get; }
        public string buy { set; get; }
        public string sold { set; get; }
        public string total { set; get; }
        public string DealerType { set; get; }

        public string getSql()
        {
            return "insert into Dealer (StockId,DealerName,buy,sold,[total],SerialNo,Date,DealerType) values('" + StockId + "','" + DealerName + "'," + buy + "," + sold + "," + total + "," + SerialNo + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DealerType+ "')" ;
        }
    }

    public class clsDealer
    {
        public string StockId { set; get; }
        public string SerialNo { set; get; }
        public ArrayList dealerObjArr = new ArrayList();

        public void GetData()
        {
            try
            {
                WebClient client = new WebClient();
                MemoryStream ms = new MemoryStream(client.DownloadData("https://tw.stock.yahoo.com/d/s/major_"+StockId+".html"));

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(ms, Encoding.Default);

                HtmlAgilityPack.HtmlDocument docStockContext = new HtmlAgilityPack.HtmlDocument();

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table")[9].SelectNodes("tr");

                int i = 0;
                clsDealerObj dobj = null;
                foreach (HtmlNode tr in nodes)
                {
                    if (i < 1)
                    {
                        i++;
                        continue;
                    }
                    HtmlNodeCollection td = tr.SelectNodes("td");
                    dobj = new clsDealerObj();
                    dobj.StockId = StockId;
                    dobj.SerialNo = SerialNo;
                    dobj.DealerName = td[0].InnerText.Trim();
                    dobj.buy = td[1].InnerText.Trim();
                    dobj.sold = td[2].InnerText.Trim();
                    dobj.total  = td[3].InnerText.Trim();
                    dobj.DealerType = "0";

                    if (td[3].InnerText.Trim() != "0" && td[3].InnerText.Trim() != "")
                        dealerObjArr.Add(dobj);

                    dobj = new clsDealerObj();
                    dobj.StockId = StockId;
                    dobj.SerialNo = SerialNo;
                    dobj.DealerName = td[4].InnerText.Trim();
                    dobj.buy = td[5].InnerText.Trim();
                    dobj.sold = td[6].InnerText.Trim();
                    dobj.total = td[7].InnerText.Trim();
                    dobj.DealerType = "1";
                    if (td[7].InnerText.Trim() != "0" && td[7].InnerText.Trim() != "")
                    dealerObjArr.Add(dobj);
                    
                }

                doc = null;
                docStockContext = null;
                client.Dispose();
                client = null;
                ms.Close();

                string sql = "";
                SQLiteTransaction trans = null;
                try
                {
                    
                    using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();

                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            foreach (clsDealerObj obj in dealerObjArr)
                            {
                                sql = obj.getSql();
                                cmd.CommandText = sql;

                                cmd.ExecuteNonQuery();
                            }
                            trans.Commit();

                        }
                    }
                }
                catch
                {
                    trans.Rollback();
                }
                
            }
            catch
            {
               
            }

          
        }
    }
}
