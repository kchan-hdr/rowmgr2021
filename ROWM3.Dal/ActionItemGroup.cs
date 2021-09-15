using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ROWM.Dal
{
    [Table("Action_Item_Group", Schema = "ROWM" )]
    public class ActionItemGroup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActionItemGroupId { get; set; }

        [StringLength(50)]
        public string GroupName { get; set; }

        [StringLength(200)]
        public string GroupNameCaption { get; set; }

        [StringLength(200)]
        public string Company { get; set; }

        public int DisplayOrder { get; set; }

        public bool CanCreateItem { get; set; } = true;
        public bool CanPerformItem { get; set; } = true;
        public bool CanCloseItem { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;

        [StringLength(50)]
        public string ModifiedBy { get; set; } = "ROWM_DEV";


        // assigned items
        public virtual ICollection<ActionItem> Assignments { get; private set; } = new HashSet<ActionItem>();
    }
}
