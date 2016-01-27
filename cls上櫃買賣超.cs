using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using System.Collections;
using System.Net;

namespace yahooFinance
{
    public class cls上櫃買賣超
    {
        static public bool 取得三大法人買賣超(out ArrayList 外資買賣超, out ArrayList 投信買賣超, out ArrayList 自營商買賣超,DateTime assignDate)
        {
            外資買賣超 = new ArrayList();
            投信買賣超 = new ArrayList();
            自營商買賣超 = new ArrayList();

            string Date = (assignDate.Year - 1911).ToString("000") + "/" + assignDate.Month.ToString("00") + "/" + assignDate.Day.ToString("00");
            MemoryStream ms = getMemoryStreamByUrl("http://www.tpex.org.tw/web/stock/3insti/daily_trade/3itrade_hedge_print.php?l=zh-tw&se=EW&t=D&d=" + Date + "&s=0,asc,0");

            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.UTF8);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table/tbody/tr"); //外資

            if (nodes == null)
            {
                return false;
                ms.Close();
            }

            cls買賣超物件 外資 = null;
            cls買賣超物件 投信 = null;
            cls買賣超物件 自營商 = null;

            foreach (HtmlNode n in nodes)
            {
                HtmlNodeCollection tdNodes = n.SelectNodes("td");
                外資 = new cls買賣超物件();
                外資.StockId = tdNodes[0].InnerText.Trim();
                外資.Total = Decimal.Parse(tdNodes[4].InnerText.Trim());
                外資買賣超.Add(外資);

                投信 = new cls買賣超物件();
                投信.StockId = tdNodes[0].InnerText.Trim();
                投信.Total = Decimal.Parse(tdNodes[7].InnerText.Trim());
                投信買賣超.Add(投信);

                自營商 = new cls買賣超物件();
                自營商.StockId = tdNodes[0].InnerText.Trim();
                自營商.Total = Decimal.Parse(tdNodes[8].InnerText.Trim());
                自營商買賣超.Add(自營商);
            }
            ms.Close();
            return true;
        }

        static private MemoryStream getMemoryStreamByUrl(string Url)
        {
            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData(Url));

            return ms;
        }
    }
}
