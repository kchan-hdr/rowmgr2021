using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Agent", Schema ="ROWM")]
    public class Agent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AgentId { get; set; }

        [Required, StringLength(20)]
        public string AgentName { get; set; }

        public virtual ICollection<ContactLog> Logs { get; set; }


        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
