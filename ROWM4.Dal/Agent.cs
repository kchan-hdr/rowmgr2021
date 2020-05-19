using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class Agent
    {
        public Agent()
        {
            Appraisal = new HashSet<Appraisal>();
            ContactLog = new HashSet<ContactLog>();
            DocumentActivity = new HashSet<DocumentActivity>();
            DocumentAgents = new HashSet<DocumentAgents>();
            StatusActivity = new HashSet<StatusActivity>();
        }

        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Appraisal> Appraisal { get; set; }
        public ICollection<ContactLog> ContactLog { get; set; }
        public ICollection<DocumentActivity> DocumentActivity { get; set; }
        public ICollection<DocumentAgents> DocumentAgents { get; set; }
        public ICollection<StatusActivity> StatusActivity { get; set; }
    }
}
