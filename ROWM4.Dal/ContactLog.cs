using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class ContactLog
    {
        public ContactLog()
        {
            ContactFollowupChildContactLog = new HashSet<ContactFollowup>();
            ContactFollowupParentContactLog = new HashSet<ContactFollowup>();
            ContactInfoContactLogs = new HashSet<ContactInfoContactLogs>();
            Document = new HashSet<Document>();
            ParcelContactLogs = new HashSet<ParcelContactLogs>();
        }

        public Guid ContactLogId { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public Guid ContactAgentId { get; set; }
        public string ContactChannel { get; set; }
        public string ProjectPhase { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public Guid? OwnerOwnerId { get; set; }
        public int? LandownerScore { get; set; }
        public bool IsDeleted { get; set; }

        public Agent ContactAgent { get; set; }
        public Owner OwnerOwner { get; set; }
        public ICollection<ContactFollowup> ContactFollowupChildContactLog { get; set; }
        public ICollection<ContactFollowup> ContactFollowupParentContactLog { get; set; }
        public ICollection<ContactInfoContactLogs> ContactInfoContactLogs { get; set; }
        public ICollection<Document> Document { get; set; }
        public ICollection<ParcelContactLogs> ParcelContactLogs { get; set; }
    }
}
