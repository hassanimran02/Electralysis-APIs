using System;
using System.Collections.Generic;

namespace Macroeconomics.DAL.Entities;

public partial class EconomicIndicatorValue
{
    public int EconomicIndicatorValueID { get; set; }

    public int EconomicIndicatorID { get; set; }

    public short EconomicIndicatorFieldID { get; set; }

    public string ValueEn { get; set; } = null!;

    public string ValueAr { get; set; } = null!;

    public string? NoteEn { get; set; }

    public string? NoteAr { get; set; }

    public virtual EconomicIndicator EconomicIndicator { get; set; } = null!;

    public virtual EconomicIndicatorField EconomicIndicatorField { get; set; } = null!;
}
