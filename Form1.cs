
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Data.SQLite;
using System.Collections;
using HtmlAgilityPack;
using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;

namespace yahooFinance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

      

        private void button1_Click(object sender, EventArgs e)
        {
           
            DataTable tb = new DataTable();
            SQLiteDataAdapter ad = new SQLiteDataAdapter("select StockId from stock where stockType='1'", ConfigurationSettings.AppSettings["connectionString"]);

            ad.Fill(tb);

            ArrayList sqlArr = new ArrayList();

            stockPrice s = null;
            System.Threading.Thread t = null;
            foreach (DataRow r in tb.Rows)
            {
                s = new stockPrice(r["StockId"].ToString().Trim());
             
                t = new System.Threading.Thread(s.GetStockInfo);
                t.Start();
                System.Threading.Thread.Sleep(600);
            }

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable tb = new DataTable();

            SQLiteDataAdapter ad = new SQLiteDataAdapter("select distinct max(serialNo) + 1 from StockPrice", ConfigurationSettings.AppSettings["connectionString"]);

            ad.Fill(tb);
            string SerialNo = tb.Rows[0][0].ToString().Trim();
            tb.Dispose();
            tb = new DataTable();
            ad = new SQLiteDataAdapter("select StockId from Stock",ConfigurationSettings.AppSettings["connectionString"]);

            ad.Fill(tb);

            StockProcess  s = null;
            System.Threading.Thread t = null;

            foreach (DataRow r in tb.Rows)
            {
                s = new StockProcess();
                s.SerialNo = SerialNo;
                s.股票代號 = r["StockId"].ToString().Trim();
                t = new System.Threading.Thread(s.Process);
                t.Start();
                System.Threading.Thread.Sleep(200);
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        
       
        private void button4_Click(object sender, EventArgs e)
        {
            StockNo stock = new StockNo();
            ArrayList arr = stock.GetAllStock();
            ArrayList SqlArr = new ArrayList();


            DataTable tb = new DataTable();
            SQLiteDataAdapter ad = new SQLiteDataAdapter("select * from Stock",ConfigurationSettings.AppSettings["connectionString"]);
            ad.Fill(tb);


            foreach (StockNo s in arr)
            {
                if (tb.Select("StockId='" + s.StockNumber + "'").Length == 0)
                {
                    SqlArr.Add("insert into Stock(StockId,StockName,UpdateDate,StockType) values('" + s.StockNumber + "','" + s.StockName + "','" + DateTime.Now.ToString("yyyyMMdd") + "','" + s.StockType + "')");
                }
                else
                {
                    SqlArr.Add("update Stock set UpdateDate='" + DateTime.Now.ToString("yyyyMMdd") + "' where StockId='" + s.StockNumber + "'");
                }
            }
            SqlArr.Add("delete from stock where UpdateDate <> '" + DateTime.Now.ToString("yyyyMMdd") + "'");



            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
            {
                conn.Open();
                SQLiteTransaction trans = conn.BeginTransaction();

                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        foreach (string sql in SqlArr)
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            ArrayList sqlArr = new ArrayList();

            sqlArr.Add("delete from Juristic  where Date='"+DateTime.Now.ToString("yyyy-MM-dd")+"'");
            sqlArr.Add("insert into Juristic select StockId,0,0,0,0, date('now'),(select max(SerialNo)+1 from Juristic) from Stock");

            ArrayList 外資 = null;
            ArrayList 投信 = null;
            ArrayList 自營商 = null;

            cls上櫃買賣超.取得三大法人買賣超(out 外資, out 投信, out 自營商,DateTime.Now);

            ArrayList fs = 上市買賣超.getForeign(DateTime.Now);

            ArrayList invest = 上市買賣超.getInvestment(DateTime.Now);

            ArrayList Dealer = 上市買賣超.getDealer(DateTime.Now);

            foreach (cls買賣超物件 f in 外資)
            {
                sqlArr.Add("update Juristic set [foreign] = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
            }

            foreach (cls買賣超物件 f in 投信)
            {
                sqlArr.Add("update Juristic set Investment = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
            }

            foreach (cls買賣超物件 f in 自營商)
            {
                sqlArr.Add("update Juristic set Dealer = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
            }

            foreach (cls買賣超物件 f in fs)
            {
                sqlArr.Add("update Juristic set [foreign] = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
            }

            foreach (cls買賣超物件 f in invest)
            {
                sqlArr.Add("update Juristic set Investment = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
            }

            foreach (cls買賣超物件 f in Dealer)
            {
                sqlArr.Add("update Juristic set Dealer = " + f.Total + " where StockId='" + f.StockId + "'  and Date='"+DateTime.Now.ToString("yyyy-MM-dd")+"'");
            }

            sqlArr.Add("update Juristic set [Total]=[foreign] + Investment + Dealer where Date='"+DateTime.Now.ToString("yyyy-MM-dd")+"'");

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
            {
                conn.Open();
                SQLiteTransaction trans = conn.BeginTransaction();

                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        foreach (string sql in sqlArr)
                        {
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable tb = new DataTable();
            SQLiteDataAdapter ad = new SQLiteDataAdapter("select StockId from stock where stockType='2'", ConfigurationSettings.AppSettings["connectionString"]);

            ad.Fill(tb);

            ArrayList sqlArr = new ArrayList();

            stockPrice s = null;
            System.Threading.Thread t = null;
            foreach (DataRow r in tb.Rows)
            {
                s = new stockPrice(r["StockId"].ToString().Trim());

                t = new System.Threading.Thread(s.GetStockInfo上櫃);
                t.Start();
                System.Threading.Thread.Sleep(600);
            }


          
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DateTime before = DateTime.Parse("2016/01/14");
            while (before <= DateTime.Parse("2016/01/18"))
            {
                ArrayList sqlArr = new ArrayList();

                sqlArr.Add("delete from Juristic  where Date='" + before.ToString("yyyy-MM-dd") + "'");
                sqlArr.Add("insert into Juristic select StockId,0,0,0,0, '" + before.ToString("yyyy-MM-dd") + "',0 from Stock where StockType='2'");

                ArrayList 外資 = null;
                ArrayList 投信 = null;
                ArrayList 自營商 = null;

                if (!cls上櫃買賣超.取得三大法人買賣超(out 外資, out 投信, out 自營商, before))
                {
                    sqlArr.Clear();
                    before = before.AddDays(1);
                    continue;
                }

                

                foreach (cls買賣超物件 f in 外資)
                {
                    sqlArr.Add("update Juristic set [foreign] = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + before.ToString("yyyy-MM-dd") + "'");
                }

                foreach (cls買賣超物件 f in 投信)
                {
                    sqlArr.Add("update Juristic set Investment = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + before.ToString("yyyy-MM-dd") + "'");
                }

                foreach (cls買賣超物件 f in 自營商)
                {
                    sqlArr.Add("update Juristic set Dealer = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + before.ToString("yyyy-MM-dd") + "'");
                }


                sqlArr.Add("update Juristic set [Total]=[foreign] + Investment + Dealer where Date='" + before.ToString("yyyy-MM-dd") + "'");

                using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
                {
                    conn.Open();
                    SQLiteTransaction trans = conn.BeginTransaction();

                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        try
                        {
                            foreach (string sql in sqlArr)
                            {
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                        }
                    }
                }
                sqlArr.Clear();
                before = before.AddDays(1);
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ArrayList sqlArr = new ArrayList();


            DateTime before = DateTime.Parse("2016/01/14");
            while (before <= DateTime.Parse("2016/01/18"))
            {
                sqlArr.Add("delete from  Juristic where StockId in ( select StockId from Stock where StockType='1') and Date='" + before.ToString("yyyy-MM-dd") + "'");
                sqlArr.Add("insert into Juristic select StockId,0,0,0,0, '" + before.ToString("yyyy-MM-dd") + "',(select ifnull(max(serialNo) + 1,1) from Juristic) from Stock where StockType='1'");

                ArrayList fs = 上市買賣超.getForeign(before);

                if (fs == null)
                {
                    sqlArr.Clear();
                    before = before.AddDays(1);
                    continue;
                } 

                ArrayList invest = 上市買賣超.getInvestment(before);

                ArrayList Dealer = 上市買賣超.getDealer(before);

                foreach (cls買賣超物件 f in fs)
                {
                    sqlArr.Add("update Juristic set [foreign] = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + before.ToString("yyyy-MM-dd") + "'");
                }

                foreach (cls買賣超物件 f in invest)
                {
                    sqlArr.Add("update Juristic set Investment = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + before.ToString("yyyy-MM-dd") + "'");
                }

                foreach (cls買賣超物件 f in Dealer)
                {
                    sqlArr.Add("update Juristic set Dealer = " + f.Total + " where StockId='" + f.StockId + "'  and Date='" + before.ToString("yyyy-MM-dd") + "'");
                }

                sqlArr.Add("update Juristic set [Total]=[foreign] + Investment + Dealer where Date='" + before.ToString("yyyy-MM-dd") + "'");

                using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
                {
                    conn.Open();
                    SQLiteTransaction trans = conn.BeginTransaction();

                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        try
                        {
                            foreach (string sql in sqlArr)
                            {
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                        }
                    }
                }
                before = before.AddDays(1);
                sqlArr.Clear();
            }
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DataTable tb = new DataTable();

            SQLiteDataAdapter ad = new SQLiteDataAdapter("select distinct max(serialNo) + 1 from Dealer", ConfigurationSettings.AppSettings["connectionString"]);

            ad.Fill(tb);

            string SerialNo = tb.Rows[0][0].ToString().Trim();

            tb.Dispose();
            tb = new DataTable();

            ad = new SQLiteDataAdapter("select StockId from Stock",ConfigurationSettings.AppSettings["connectionString"]);

            ad.Fill(tb);
            clsDealer  c = null;
            System.Threading.Thread t = null;

            //c = new clsDealer();
            //c.SerialNo = SerialNo;
            //c.StockId = "5478";
            //t = new System.Threading.Thread(c.GetData);
            //t.Start();

            foreach (DataRow r in tb.Rows)
            {
                c = new clsDealer();
                c.SerialNo = SerialNo;
                c.StockId = r["StockId"].ToString().Trim();
                t = new System.Threading.Thread(c.GetData);
                t.Start();
                System.Threading.Thread.Sleep(200);
            }
        

        }

        private void button9_Click(object sender, EventArgs e)
        {
            DataTable tb = new DataTable();
            SQLiteDataAdapter ad = new SQLiteDataAdapter("SELECT distinct [Date] FROM [Juristic] order by [Date] ", ConfigurationSettings.AppSettings["connectionString"]);

            ad.Fill(tb);

            int i = 1;

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
            {
                conn.Open();
               

                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        foreach(DataRow r in tb.Rows)
                        {
                            cmd.CommandText = "update Juristic set SerialNo=" + i.ToString() + " where [Date] ='" + DateTime.Parse(r["Date"].ToString().Trim()).ToString("yyyy-MM-dd") + "'";
                            cmd.ExecuteNonQuery();
                            i++;
                        }
                    }
                    catch
                    {
                        
                    }
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DataTable tb = new DataTable();
            SQLiteDataAdapter ad = new SQLiteDataAdapter("SELECT distinct [Date] FROM [StockPrice] order by [Date] ", ConfigurationSettings.AppSettings["connectionString"]);

            ad.Fill(tb);

            int i = 1;

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
            {
                conn.Open();


                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        foreach (DataRow r in tb.Rows)
                        {
                            cmd.CommandText = "update StockPrice set SerialNo=" + i.ToString() + " where [Date] ='" + DateTime.Parse(r["Date"].ToString().Trim()).ToString("yyyy-MM-dd") + "'";
                            cmd.ExecuteNonQuery();
                            i++;
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

      
    }
}
