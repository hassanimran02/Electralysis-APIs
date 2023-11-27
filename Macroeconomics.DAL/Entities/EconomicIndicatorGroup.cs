using System;
using System.Collections.Generic;

namespace Macroeconomics.DAL.Entities;

public partial class EconomicIndicatorGroup
{
    public int GroupID { get; set; }

    public string NameEn { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public int? ParentGroupID { get; set; }

    public int DisplaySeqNo { get; set; }

    public virtual ICollection<EconomicIndicatorField> EconomicIndicatorFields { get; set; } = new List<EconomicIndicatorField>();

    public virtual ICollection<EconomicIndicatorGroup> InverseParentGroup { get; set; } = new List<EconomicIndicatorGroup>();

    public virtual EconomicIndicatorGroup? ParentGroup { get; set; }
}
