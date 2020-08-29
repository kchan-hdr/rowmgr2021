﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("DocumentActivity", Schema = "ROWM")]
    public partial class DocumentActivity
    {
        [Key]
        public Guid ActivityId { get; set; }
        public DateTimeOffset ActivityDate { get; set; }
        [Required]
        [StringLength(100)]
        public string Activity { get; set; }
        public string ActivityNotes { get; set; }
        public Guid ParentDocumentId { get; set; }
        public Guid? ChildDocumentId { get; set; }
        public Guid AgentId { get; set; }

        [ForeignKey(nameof(AgentId))]
        [InverseProperty("DocumentActivity")]
        public virtual Agent Agent { get; set; }
        [ForeignKey(nameof(ChildDocumentId))]
        [InverseProperty(nameof(Document.DocumentActivityChildDocument))]
        public virtual Document ChildDocument { get; set; }
        [ForeignKey(nameof(ParentDocumentId))]
        [InverseProperty(nameof(Document.DocumentActivityParentDocument))]
        public virtual Document ParentDocument { get; set; }
    }
}
