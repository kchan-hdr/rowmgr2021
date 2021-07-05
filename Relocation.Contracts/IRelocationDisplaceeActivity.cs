using System;

namespace com.hdr.rowmgr.Relocation
{
    public enum DisplaceeActivity { unk = 0, sent, delivered, toConsultant, toClient, clientApproved }

    public interface IRelocationDisplaceeActivity
    {
        Guid AgentId { get; set; }

        string ActivityDescription { get; set; }
        DisplaceeActivity Activity { get; set; }
        DateTimeOffset ActivityDate { get; set; }
        string Notes { get; set; }
    }
}
