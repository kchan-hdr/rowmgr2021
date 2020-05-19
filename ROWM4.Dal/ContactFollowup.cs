using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ContactFollowup
    {
        public Guid Id { get; set; }
        public int FollowupType { get; set; }
        public Guid ParentContactLogId { get; set; }
        public Guid ChildContactLogId { get; set; }

        public ContactLog ChildContactLog { get; set; }
        public ContactLog ParentContactLog { get; set; }
    }
}
