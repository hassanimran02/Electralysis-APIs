using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Models.EconomicIndicatorSource
{
    public class EconomicIndicatorSourceModel
    {
        [SwaggerSchema(ReadOnly = true)]
        public short EconomicIndicatorSourceId { get; set; }
        [Required]
        public short CountryId { get; set; } 
        public string EISourceNameEn { get; set; } = null!;
        public string EISourceNameAr { get; set; } = null!;
    }
}
