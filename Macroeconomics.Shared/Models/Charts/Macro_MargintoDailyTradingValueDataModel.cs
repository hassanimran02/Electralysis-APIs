using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Models.Charts
{
    public class Macro_MargintoDailyTradingValueDataModel
    {
        public int FiscalPeriodTypeID { get; set; }
        public string fp { get; set; }
        public int marketid { get; set; }
        public DateTime FDDate { get; set; }
        public int y { get; set; }
        public string m { get; set; }
        public decimal tvolume { get; set; }
        public decimal tvolumemqtravg { get; set; }
        public decimal tvolumemyearlyavg { get; set; }
    }
}
