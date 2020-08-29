﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROWM.Dal
{
    [Table("ContactInfoContactLogs", Schema = "ROWM")]
    public partial class ContactInfoContactLogs
    {
        [Key]
        [Column("ContactInfo_ContactId")]
        public Guid ContactInfoContactId { get; set; }
        [Key]
        [Column("ContactLog_ContactLogId")]
        public Guid ContactLogContactLogId { get; set; }

        [ForeignKey(nameof(ContactInfoContactId))]
        [InverseProperty(nameof(ContactInfo.ContactInfoContactLogs))]
        public virtual ContactInfo ContactInfoContact { get; set; }
        [ForeignKey(nameof(ContactLogContactLogId))]
        [InverseProperty(nameof(ContactLog.ContactInfoContactLogs))]
        public virtual ContactLog ContactLogContactLog { get; set; }
    }
}
