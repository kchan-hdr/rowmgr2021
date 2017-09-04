using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class Status_Master
    {
        [Key, StringLength(40)]
        public string Code { get; set; }

        public int DomainValue { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    [Table("Parcel_Status", Schema ="ROWM")]
    public class ParcelStatus_Master : Status_Master { }

    [Table("Roe_Status", Schema = "ROWM")]
    public class RoeStatus_Master : Status_Master { }

}
