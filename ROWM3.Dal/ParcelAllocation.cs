using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Parcel_Allocation", Schema = "ROWM")]
    public class ParcelAllocation
    {
        [Key]
        public Guid AllocationId { get; private set; }

        public int ProjectPartId { get; private set; }
        [ForeignKey(nameof(ProjectPartId))]
        public virtual ProjectPart ProjectPart { get; set; }

        public Guid ParcelId { get; private set; }
        public string TrackingNumber { get; set; }

        [ForeignKey(nameof(ParcelId))]
        public virtual Parcel Parcel { get; set; }

        public bool IsActive { get; private set; }
    }
}
