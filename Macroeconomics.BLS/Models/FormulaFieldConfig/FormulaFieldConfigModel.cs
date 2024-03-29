﻿using Swashbuckle.AspNetCore.Annotations;

namespace Macroeconomics.BLS.Models.FormulaFieldConfig
{
    public class FormulaFieldConfigModel
    {
        public int FieldConfigId { get; set; }
        public string FieldNameEn { get; set; }
        public string FieldNameAr { get; set; }
        public string Formula { get; set; }
        public string FormulaFieldIDs { get; set; }
        public int? UnitId { get; set; }
        public string DataSource { get; set; }
        public string Description { get; set; }
        public string? ConstantValues { get; set; }
        public string FormulaType { get; set; }
    }
}
