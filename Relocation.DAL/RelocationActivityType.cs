using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.DAL
{
    [Table("Relocation_Activity_Type", Schema ="Austin")]
    public partial class RelocationActivityType
    {
        [Key, StringLength(20)]
        public string ActivityTypeCode { get; set; }

        [StringLength(200)]
        public string Description { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public bool TrackSent { get; set; } = false;
        public bool TrackDelivered { get; set; } = false;
        public bool TrackSentConsultant { get; set; } = false;
        public bool TrackSentClient { get; set; } = false;
        public bool TrackClientApproval { get; set; } = false;

        public bool IncludeYesNo { get; set; }
        public bool IncludeMoney { get; set; }
    }
}
