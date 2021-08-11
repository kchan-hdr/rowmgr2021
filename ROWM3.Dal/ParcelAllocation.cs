using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Parcel_Allocation", Schema = "ROWM")]
    public class ParcelAllocation
    {
        [Key]
        public Guid AllocationId { get; private set; }

        public int ProjectPartId { get; private set; }
        public Guid ParcelId { get; private set; }

        public string TrackingNumber { get; set; }

        public bool IsActive { get; private set; }
    }
}
