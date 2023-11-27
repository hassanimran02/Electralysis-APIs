using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Models.EconomicIndicatorValue
{
    public class EconomicIndicatorValueModel
    {
        [SwaggerSchema(ReadOnly = true)]
        public int EconomicIndicatorValueId { get; set; }
        public int EconomicIndicatorId { get; set; }
        public short EconomicIndicatorFieldId { get; set; }
        [Required]
        public decimal ValueEn { get; set; }
        [Required]
        public decimal ValueAr { get; set; }    
        public string? NoteEn { get; set; }
        public string? NoteAr { get; set; }
    }
}
