using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Models.Charts
{
    public class ArgaamSectorsModel
    {
        public int ArgaamSectorID { get; set; }
        public int? ParentArgaamSectorID { get; set; }
        public string ArgaamSectorNameAr { get; set; }
        public string ArgaamSectorNameEn { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public int DisplaySeqNo { get; set; }
        public int CompanyArgaamSectorID { get; set; }
        public int CompanyID { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDisplay { get; set; }
    }
}
