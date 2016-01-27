using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;

namespace yahooFinance
{
    public class StockProcess
    {
        public static object obj = new object();
        public string 股票代號 { set; get; }
        public string SerialNo { set; get; }

        public void Process()
        {
            Stock s = new Stock();
            s.股票代號 = 股票代號;
            s.SerialNo = SerialNo;
            if (s.GetStockInfo())
            {
                //lock (obj)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(ConfigurationSettings.AppSettings["connectionString"]))
                    {
                        conn.Open();

                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            try
                            {
                                cmd.CommandText = s.GetSqlString();

                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                string ss = "";
                            }
                        }
                    }
                }
               
            }
        }

    }
}
