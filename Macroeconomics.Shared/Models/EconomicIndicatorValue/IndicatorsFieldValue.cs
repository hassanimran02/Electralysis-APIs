using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Models.EconomicIndicatorValue
{
    public class IndicatorsFieldValue
    {
        public short EconomicIndicatorFieldId { get; set; }
        public string DisplayNameAr { get; set; } = string.Empty;
        public string DisplayNameEn { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int Year { get; set; }
        public short FiscalPeriodID { get; set; }
        public int FiscalPeriodTypeID { get; set; }
    }
}
