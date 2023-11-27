using Swashbuckle.AspNetCore.Annotations;

namespace Macroeconomics.Shared.Models.EconomicIndicatorField
{
    public class EconomicIndicatorFieldModel
    {
        [SwaggerSchema(ReadOnly = true)]
        public short EconomicIndicatorFieldId { get; set; }

        public string DisplayNameEn { get; set; } = null!;

        public string DisplayNameAr { get; set; } = null!;

        public short DisplaySeqNo { get; set; }

        public short? MeasuringUnitId { get; set; }

        public int GroupId { get; set; }

        public bool? IsChart { get; set; }
    }
}
