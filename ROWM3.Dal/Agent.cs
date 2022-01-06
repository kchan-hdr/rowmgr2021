namespace ROWM.Dal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ROWM.Agent")]
    public partial class Agent
    {
        public Agent()
        {
            ContactLog = new HashSet<ContactLog>();
            DocumentActivity = new HashSet<DocumentActivity>();
            Document = new HashSet<Document>();
        }

        public Guid AgentId { get; set; }

        [Required]
        [StringLength(20)]
        public string AgentName { get; set; }

        public Guid? AADObjectId { get; set; }

        [StringLength(200)]
        public string AgentEmail { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastModified { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<ContactLog> ContactLog { get; set; }

        public virtual ICollection<DocumentActivity> DocumentActivity { get; set; }

        public virtual ICollection<Document> Document { get; set; }
    }
}
