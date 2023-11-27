using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Models.MacroEconomics
{
    public class PP_Indicator_FormulaDataModel
    {
        public string EntityID { get; set; } = "";  
        public string EntityName { get; set; } = "";
        public string ForYear { get; set; } = "";
        public string IndicatorValues { get; set; } = "";
        public string PrevValue { get; set; } = "";
        public string FiscalPeriodValue { get; set; } = "";
        public string Formula { get; set; } = "";
        public string FormulaValue { get; set; } = "";
        public string CalulatedValue { get; set; } = "";
        public string FormulaIds { get; set; } = "";
        public string ConstantValues { get; set; } = "";
        public string FormulaType { get; set; } = "";
        public string Labels { get; set; } = "";
        public DateTime? ForDate { get; set; }
    }
}
