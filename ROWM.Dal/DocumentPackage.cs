using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("DocumentPackage", Schema ="ROWM")]
    public class DocumentPackage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PackageId { get; set; }

        [Required, StringLength(100)]
        public string PackageName { get; set; }

        public virtual ICollection<Document> Documents { get; set; }


        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
