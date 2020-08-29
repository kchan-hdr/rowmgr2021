﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("Ownership", Schema = "ROWM")]
    public partial class Ownership
    {
        [Key]
        public Guid OwnershipId { get; set; }
        public Guid? ParcelId { get; set; }
        public Guid OwnerId { get; set; }
        [Column("Ownership_t")]
        public int OwnershipT { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [ForeignKey(nameof(OwnerId))]
        [InverseProperty("Ownership")]
        public virtual Owner Owner { get; set; }
        [ForeignKey(nameof(ParcelId))]
        [InverseProperty("Ownership")]
        public virtual Parcel Parcel { get; set; }
    }
}
