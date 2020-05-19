using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class DocumentAgents
    {
        public Guid AgentAgentId { get; set; }
        public Guid DocumentDocumentId { get; set; }

        public Agent AgentAgent { get; set; }
        public Document DocumentDocument { get; set; }
    }
}
