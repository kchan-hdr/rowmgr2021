using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Status_Category", Schema ="ROWM")]
    public class StatusCategory
    {
        [Key]
        public string CategoryCode { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Parcel_Status> Parcel_Statuses { get; set; }
    }
}
