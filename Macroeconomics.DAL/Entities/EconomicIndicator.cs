using System;
using System.Collections.Generic;

namespace Macroeconomics.DAL.Entities;

public partial class EconomicIndicator
{
    public int EconomicIndicatorID { get; set; }

    public short? CountryID { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsPublished { get; set; }

    public int ForYear { get; set; }

    public short FiscalPeriodID { get; set; }

    public int? SubGroupID { get; set; }

    public virtual ICollection<EconomicIndicatorValue> EconomicIndicatorValues { get; set; } = new List<EconomicIndicatorValue>();
}
