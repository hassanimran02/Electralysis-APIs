using System;
using System.Collections.Generic;

namespace Macroeconomics.DAL.Entities;

public partial class EconomicIndicatorField
{
    public short EconomicIndicatorFieldID { get; set; }

    public string DisplayNameEn { get; set; } = null!;

    public string DisplayNameAr { get; set; } = null!;

    public short DisplaySeqNo { get; set; }

    public short? MeasuringUnitID { get; set; }

    public int GroupID { get; set; }

    public bool? IsChart { get; set; }

    public virtual ICollection<EconomicIndicatorValue> EconomicIndicatorValues { get; set; } = new List<EconomicIndicatorValue>();

    public virtual EconomicIndicatorGroup Group { get; set; } = null!;
}
