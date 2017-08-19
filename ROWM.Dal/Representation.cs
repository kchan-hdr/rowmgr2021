using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Repesentation_Type", Schema ="ROWM")]
    public class Representation
    {
        [Key, StringLength(20)]
        public string RelationTypeCode { get; set; }

        [Required,StringLength(20)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
