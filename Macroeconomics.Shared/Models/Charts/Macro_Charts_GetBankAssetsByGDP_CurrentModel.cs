using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Models.Charts
{
    public class Macro_Charts_GetBankAssetsByGDP_CurrentModel
    {
        public int AttributeId { get; set; }
        public int SectorId { get; set; }
        public string SectorNameEn { get; set; }
        public string DisplayName { get; set; }
        public int ForYear { get; set; }
        public short FiscalPeriodID { get; set; }
        public string FiscalPeriodValue { get; set; }
        public decimal Value { get; set; }
    }
}
