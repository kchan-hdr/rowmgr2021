using com.hdr.rowmgr.Relocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Relocation.Tests
{
    [TestClass]
    public class DisplaceeTests
    {
        readonly Guid TEST_PARCEL = Guid.NewGuid();

        [TestMethod]
        public async Task Normal_Activity()
        {
            var c = new RelocationContext();
            var r = new RelocationRepository(c);
            IRelocationCaseOps h = new RelocationCaseOps(c, r);

            var p = await h.AddRelocationCase(TEST_PARCEL, "activity test", RelocationStatus.Unk, DisplaceeType.Landlord, RelocationType.PersonalProperty);

            var ca = p.RelocationCases.Last();

            var random = new Random();

            foreach(var t in await h.GetActivityTypes())
            foreach(var tt in t.GetTasks())
            {
                //await h.AddActivity(ca.RelocationCaseId, t.ActivityTypeCode, tt, Guid.NewGuid(), DateTimeOffset.UtcNow, t.Description);
                //t.IncludeMoney ? random.Next(100,1000) : null,
                //t.IncludeYesNo ? true : null );

                if (t.IncludeMoney)
                    await h.AddActivity(ca.RelocationCaseId, tt.ActivityTypeCode, tt.Activity, tt.Caption, Guid.NewGuid(), DateTimeOffset.UtcNow, t.Description, money: random.Next(100,1000));
                else if (t.IncludeYesNo)
                    await h.AddActivity(ca.RelocationCaseId, tt.ActivityTypeCode, tt.Activity, tt.Caption, Guid.NewGuid(), DateTimeOffset.UtcNow, t.Description, bValue: random.Next(10) > 6);
                else
                    await h.AddActivity(ca.RelocationCaseId, tt.ActivityTypeCode, tt.Activity, tt.Caption, Guid.NewGuid(), DateTimeOffset.UtcNow, t.Description);
            }
        }

        [TestMethod]
        public void Parse_Eligibility()
        {
            Assert.IsTrue( Enum.TryParse<RelocationStatus>("ineligible", true, out var r));
        }
    }
}
