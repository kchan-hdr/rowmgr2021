﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("RoeConditions", Schema ="ROWM")]
    public class RoeCondition
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ConditionId { get; set; }
        [Required]
        public string Condition { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? EffectiveStartDate { get; set; }
        public DateTimeOffset? EffectiveEndDate { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
