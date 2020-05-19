using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ContactInfoContactLogs
    {
        public Guid ContactInfoContactId { get; set; }
        public Guid ContactLogContactLogId { get; set; }

        public ContactInfo ContactInfoContact { get; set; }
        public ContactLog ContactLogContactLog { get; set; }
    }
}
