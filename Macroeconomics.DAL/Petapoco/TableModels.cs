using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.DAL.Petapoco
{
    public class TableModels
    {
        [TableName("EconomicIndicatorGroups")]
        [PrimaryKey("GroupID")]
        [ExplicitColumns]
        public partial class EconomicIndicatorGroup
        {
            [Column]
            public int GroupID { get; set; }
            [Column]
            public string NameEn { get; set; } = null!;
            [Column]
            public string NameAr { get; set; } = null!;
            [Column]
            public int? ParentGroupID { get; set; }
            [Column]
            public int DisplaySeqNo { get; set; }
        }
    }
}
