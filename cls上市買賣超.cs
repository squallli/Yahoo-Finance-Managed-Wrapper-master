using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;

namespace yahooFinance
{
    public class 上市買賣超
    {
        static public ArrayList getForeign(DateTime date)
        {
            ArrayList Foreign = new ArrayList();

            Stream ms = getMemoryStreamByUrl("http://www.twse.com.tw/ch/trading/fund/TWT38U/TWT38U.php", date);

            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.UTF8);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table/tbody/tr"); //外資

            if (nodes == null)
            {
                ms.Close();
                return null;
            } 

            cls買賣超物件 f = null;

            foreach (HtmlNode n in nodes)
            {
                HtmlNodeCollection tdNodes = n.SelectNodes("td");
                f = new cls買賣超物件();
                f.StockId = tdNodes[1].InnerText.Trim();
                f.Buy = Decimal.Parse(tdNodes[3].InnerText);
                f.Sold = Decimal.Parse(tdNodes[4].InnerText);
                f.Total = Decimal.Parse(tdNodes[5].InnerText);

                Foreign.Add(f);
            }

            return Foreign;
        }

        static public ArrayList getInvestment(DateTime date)
        {
            ArrayList Foreign = new ArrayList();

            Stream ms = getMemoryStreamByUrl("http://www.twse.com.tw/ch/trading/fund/TWT44U/TWT44U.php",date);

            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.UTF8);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table/tbody/tr"); //投信

            cls買賣超物件 f = null;

            foreach (HtmlNode n in nodes)
            {
                HtmlNodeCollection tdNodes = n.SelectNodes("td");
                f = new cls買賣超物件();
                f.StockId = tdNodes[1].InnerText.Trim();
                f.Buy = Decimal.Parse(tdNodes[3].InnerText);
                f.Sold = Decimal.Parse(tdNodes[4].InnerText);
                f.Total = Decimal.Parse(tdNodes[5].InnerText);

                Foreign.Add(f);
            }
            ms.Close();
            return Foreign;
        }

        static public ArrayList getDealer(DateTime date)
        {
            ArrayList Foreign = new ArrayList();

            Stream ms = getMemoryStreamByUrl("http://www.twse.com.tw/ch/trading/fund/TWT43U/TWT43U.php", date);

            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.UTF8);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table/tbody/tr"); //自營商

            cls買賣超物件 f = null;

            foreach (HtmlNode n in nodes)
            {
                HtmlNodeCollection tdNodes = n.SelectNodes("td");
                f = new cls買賣超物件();
                f.StockId = tdNodes[0].InnerText.Trim();
                f.Buy = Decimal.Parse(tdNodes[8].InnerText);
                f.Sold = Decimal.Parse(tdNodes[9].InnerText);
                f.Total = Decimal.Parse(tdNodes[10].InnerText);

                Foreign.Add(f);
            }
            ms.Close();
            return Foreign;
        }


        static private Stream getMemoryStreamByUrl(string Url,DateTime date)
        {
            string 民國年 = (date.Year - 1911).ToString() + "/" + date.Month.ToString("00") + "/" + date.Day.ToString("00");
            WebRequest request = WebRequest.Create(Url);
            request.Method = "POST";
            string paramter = "download=&qdate=" + 民國年 + "&sorting=by_issue";
            byte[] bs = System.Text.Encoding.Default.GetBytes(paramter);
            request.ContentLength = bs.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(bs, 0, bs.Length);
            dataStream.Close();
            return request.GetResponse().GetResponseStream();
        }
    }
}
