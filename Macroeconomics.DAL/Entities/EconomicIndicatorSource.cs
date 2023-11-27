using System;
using System.Collections.Generic;

namespace Macroeconomics.DAL.Entities;

public partial class EconomicIndicatorSource
{
    public short EconomicIndicatorSourceID { get; set; }

    public short CountryID { get; set; }

    public string EISourceNameEn { get; set; } = null!;

    public string EISourceNameAr { get; set; } = null!;
}
