using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Contact_Purpose", Schema ="ROWM")]
    public class Purpose_Master
    {
        [Key, StringLength(20)]
        public string PurposeCode { get; set; }

        [Required, StringLength(20)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
