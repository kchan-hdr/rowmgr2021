using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Project_Part", Schema ="ROWM")]
    public class ProjectPart
    {
        [Key]
        public int ProjectPartId { get; private set; }

        public string Caption { get; private set; }
        public int? DisplayOrder { get; private set; }

        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
    }
}
