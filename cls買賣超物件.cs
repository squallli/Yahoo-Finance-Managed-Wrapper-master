using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yahooFinance
{
    public class cls買賣超物件
    {
        public string StockId { set; get; }
        public decimal Buy { set; get; }
        public decimal Sold { set; get; }
        public decimal Total { set; get; }
        public decimal Date { set; get; }
    }
}
