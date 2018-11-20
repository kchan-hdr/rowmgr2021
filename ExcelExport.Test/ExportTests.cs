using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelExport.Test
{
    [TestClass]
    public class ExportTests
    {
        [TestMethod, TestCategory("Export")]
        public void Empty_Export()
        {
            var e = new Exporter<Object>(null);
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }

        [TestMethod, TestCategory("Export")]
        public void Agent_Log_Export()
        {
            // arrange
            var repo = new ROWM.Dal.OwnerRepository( new ROWM.Dal.ROWM_Context(ROWM.Dal.DbConnection.GetConnectionString()));
            var logs = repo.GetLogs();

            var d = logs.SelectMany(lx => lx.Parcel.Select( p => new AgentLogExport.AgentLog
            {
                agentname = lx.Agent.AgentName,
                contactchannel = lx.ContactChannel,
                dateadded = lx.DateAdded,
                notes = lx.Notes?.TrimEnd(',') ?? "",
                ownerfirstname = p.Ownership.FirstOrDefault()?.Owner.PartyName?.TrimEnd(',') ?? "",
                ownerlastname = p.Ownership.FirstOrDefault()?.Owner.PartyName?.TrimEnd(',') ?? "",
                parcelid = p.Assessor_Parcel_Number,
                parcelstatus = p.Parcel_Status.Description,
                parcelstatuscode = p.ParcelStatusCode,
                projectphase = lx.ProjectPhase,
                roestatus = p.Roe_Status.Description,
                roestatuscode = p.RoeStatusCode,
                title = lx.Title?.TrimEnd(',') ?? ""
            }));

            var e = new AgentLogExport(d);
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }

        [TestMethod, TestCategory("Export")]
        public void Contact_List_Export()
        {
            var e = new ContactListExport(null);
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }

        [TestMethod, TestCategory("Export")]
        public void Doc_List_Export()
        {
            var e = new DocListExport(null);
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }
    }
}
