using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.BLS.Models.EconomicIndicatorValue
{
    public class EconomicIndicatorValueChartModel
    {
        //public string Name { get; set; } = "";
        //public decimal Value { get; set; }
        //public int ForYear { get; set; }
        //public string FiscalPeriodValue { get; set; } = "";
        public int GroupID { get; set; }
        //public string GroupName { get; set; } = "";



        public int EconomicIndicatorValueID { get; set; }
        public int EconomicIndicatorID { get; set; }
        public short EconomicIndicatorFieldID { get; set; }
        public string NoteAr { get; set; } = "";
        public string NoteEn { get; set; } = "";
        public decimal ValueAr { get; set; }
        public decimal ValueEn { get; set; }
        public string Name { get; set; } = "";
        public decimal Value { get; set; }
        public int ForYear { get; set; }
        public string FiscalPeriodValue { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string DisplayNameAr { get; set; } = "";
        public string DisplayNameEn { get; set; } = "";
        public int DisplaySeqNo { get; set; }
        public int MeasuringUnitID { get; set; }
        public  string MeasuringUnitNameAr { get; set; } = "";
        public string MeasuringUnitNameEn { get; set; } = "";
        public bool IsChart { get; set; }

    }

    public class FiscalPeriodTypeModel1
    {
        public short FiscalPeriodTypeID { get; set; }
        public string FiscalPeriodTypeName { get; set; } = "";
    }
}