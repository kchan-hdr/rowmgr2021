using com.hdr.rowmgr.Relocation;
using ROWM.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.DAL
{
    [Table("Parcel_Relocation", Schema ="Austin")]
    public partial class ParcelRelocation
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ParcelRelocationId { get; set; }

        public Guid ParcelId { get; set; }

        public virtual ICollection<RelocationCase> Cases { get; } = new HashSet<RelocationCase>();

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
