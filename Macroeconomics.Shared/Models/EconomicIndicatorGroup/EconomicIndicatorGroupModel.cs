using Macroeconomics.Shared.Models.EconomicIndicatorField;
using Swashbuckle.AspNetCore.Annotations;

namespace Macroeconomics.Shared.Models.EconomicIndicatorGroup
{
    public class EconomicIndicatorGroupModel
    {
        [SwaggerSchema(ReadOnly = true)]
        public int GroupID { get; set; }

        public string NameEn { get; set; } = null!;

        public string NameAr { get; set; } = null!;

        public int? ParentGroupID { get; set; }

        public int DisplaySeqNo { get; set; }
    }

    public class EconomicIndicatorGroupHierarchyModel : EconomicIndicatorGroupModel
    {
        public EconomicIndicatorGroupHierarchyModel()
        {
            SubGroups = new List<EconomicIndicatorGroupModel>();
        }
        public List<EconomicIndicatorGroupModel> SubGroups { get; set; }
    }
}
