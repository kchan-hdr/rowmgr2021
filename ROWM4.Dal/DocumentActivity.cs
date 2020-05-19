using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class DocumentActivity
    {
        public Guid ActivityId { get; set; }
        public DateTimeOffset ActivityDate { get; set; }
        public string Activity { get; set; }
        public string ActivityNotes { get; set; }
        public Guid ParentDocumentId { get; set; }
        public Guid? ChildDocumentId { get; set; }
        public Guid AgentId { get; set; }

        public Agent Agent { get; set; }
        public Document ChildDocument { get; set; }
        public Document ParentDocument { get; set; }
    }
}
