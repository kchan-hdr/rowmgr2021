using com.hdr.rowmgr.Relocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Relocation.Tests
{
    [TestClass]
    public class DbTests
    {
        readonly Guid TEST_PARCEL = Guid.NewGuid();

        [TestMethod]
        public async Task Simple_Relocation()
        {
            var c = new RelocationContext();
            var r = new RelocationRepository(c);
            IRelocationCaseOps h = new RelocationCaseOps(c, r);

            foreach (RelocationStatus el in Enum.GetValues(typeof(RelocationStatus)))
            foreach (RelocationType tl in Enum.GetValues(typeof(RelocationType)))
            foreach (DisplaceeType dl in Enum.GetValues(typeof(DisplaceeType)))
            {
                var p = await h.AddRelocationCase(TEST_PARCEL, string.Empty, el, dl, tl);
                Assert.IsNotNull(p);
                Assert.AreEqual(TEST_PARCEL, p.ParcelId);
            }

            var cases = await h.GetRelocationCases(TEST_PARCEL);
            Assert.IsTrue(cases.Any());
            foreach( var cx in cases)
                Trace.WriteLine(cx.AcqFilenamePrefix);

            var newStatus = RelocationStatus.Optional;
            var rc = cases.Last();
            var p2 = await h.ChangeEligibility(rc.RelocationCaseId, newStatus, Guid.NewGuid(), DateTimeOffset.UtcNow, $"test {TEST_PARCEL}");
            rc = p2.RelocationCases.First(rx => rx.RelocationCaseId == rc.RelocationCaseId);
            Assert.AreEqual(newStatus, rc.Status);
        }

        [TestMethod, ExpectedException(typeof(System.Collections.Generic.KeyNotFoundException))]
        public async Task Bad_Parcel()
        {
            var c = new RelocationContext();
            var r = new RelocationRepository(c);
            IRelocationCaseOps h = new RelocationCaseOps(c, r);
            await h.GetRelocationCases(Guid.Empty);
        }

        [TestMethod]
        public async Task Relocation_history()
        {
            var c = new RelocationContext();
            var r = new RelocationRepository(c);
            IRelocationCaseOps h = new RelocationCaseOps(c, r);


            // pretend parcels
            var parcels = c.Relocations.Select(rx => rx.ParcelId).Distinct().ToArray();

            foreach(var parcel in parcels)
            {
                var p = await h.GetRelocation(parcel);

                Trace.WriteLine($"Parcel: {parcel}");

                foreach(var ca in p.RelocationCases)
                {
                    Trace.WriteLine($"Case {ca.AcqFilenamePrefix}");

                    var log = ca.EligibilityHistory.Select(eh => 
                        new { eh.ActivityDate, Description = $"{eh.ActivityDate}, {Enum.GetName(typeof(RelocationStatus), eh.NewStatus)} from {Enum.GetName(typeof(RelocationStatus), eh.OriginalStatus)}" })
                        .ToList();

                    log.AddRange(ca.DisplaceeActivities.Select(da =>
                        new { da.ActivityDate, Description = $"{da.ActivityDate}, {da.ActivityDescription}" }));

                    foreach (var l in log.OrderByDescending(lx => lx.ActivityDate))
                        Trace.WriteLine(l.Description);
                }
            }
        }
    }
}
