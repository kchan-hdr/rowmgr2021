using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class OwnerDocuments
    {
        public Guid DocumentDocumentId { get; set; }
        public Guid OwnerOwnerId { get; set; }

        public Document DocumentDocument { get; set; }
        public Owner OwnerOwner { get; set; }
    }
}
