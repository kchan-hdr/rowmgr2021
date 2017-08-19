using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Contact_Channel", Schema ="ROWM")]
    public class Channel_Master
    {
        [Key, StringLength(20)]
        public string ContactTypeCode { get; set; }

        [Required, StringLength(20)]
        public string Description { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
