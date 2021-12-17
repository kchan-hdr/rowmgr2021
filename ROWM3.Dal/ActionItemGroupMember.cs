using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("ACTION_ITEM_GROUP_MEMBER", Schema ="ROWM")]
    public class ActionItemGroupMember
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActionGroupMemberId { get; set; }

        [ForeignKey(nameof(ParentGroup))]
        public Guid ParentGroupId { get; set; }

        [StringLength(200)]
        public string MemberName { get; set; }

        [StringLength(200), Required, EmailAddress]
        public string Email { get; set; }

        [StringLength(50), Phone]
        public string MessageNumber { get; set; }

        public bool IsSend { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;
        public string ModifiedBy { get; set; } = "DEV";

        //
        public virtual ActionItemGroup ParentGroup { get; set; }
    }
}
