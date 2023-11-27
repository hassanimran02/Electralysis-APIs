using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Models.Charts
{
    public class PP_Indicator_FieldDataModel
    {
        public int EntityID { get; set; }
        public string EntityName { get; set; }
        public string Labels { get; set; }
        public DateTime ForDate { get; set; }
        public int ForYear { get; set; }
        public string FiscalPeriodValue { get; set; }
        public decimal Value { get; set; }
        public decimal IndicatorValue { get; set; }
    }
}
