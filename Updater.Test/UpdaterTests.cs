using AutoFixture.Xunit2;
using geographia.ags;
using Moq;
using ROWM.Controllers;
using ROWM.Dal;
using ROWM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Updater.Test
{
    public class UpdaterTests : IDisposable
    {
        readonly ITestOutputHelper _log;

        Parcel _parcel;
        List<Parcel> _inmates;
        UpdateParcelStatus _basic;
        UpdateParcelStatus2 _logic;

        UpdateParcelStatus_Ex _Ex;

        [Fact]
        public async Task Simple_Update_With_DocType()
        {
            _parcel.Parcel_Status = _context.Parcel_Status.Single(sx => sx.Code == "No_Activity");
            _parcel.ParcelStatusCode = _parcel.Parcel_Status.Code;

            var dt = _docTypes.AllTypes.First(dx => dx.DisplayOrder == 11) ?? throw new IndexOutOfRangeException();
            var dox = new Document
            {
                DocumentType = dt.DocTypeName
            };
            _parcel.Document.Add(dox);

            var touched = await _Ex.Update(_inmates, new Agent { AgentId = Guid.NewGuid(), AgentName = "BUGGER" }, "notes", DateTimeOffset.Now, dox, null);
            Assert.True(touched);

            _history.Verify(c => c.Add(It.IsAny<StatusActivity>()), Times.Once());
            _ctx.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Related_Parcels_Update_With_DocType()
        {
            _parcel.Parcel_Status = _context.Parcel_Status.Single(sx => sx.Code == "No_Activity");
            _parcel.ParcelStatusCode = _parcel.Parcel_Status.Code;

            var p2 = new Parcel
            {
                ParcelStatusCode = _parcel.ParcelStatusCode,
                Parcel_Status = _parcel.Parcel_Status
            };
            _inmates.Add(p2);

            var dt = _docTypes.AllTypes.First(dx => dx.DisplayOrder == 11) ?? throw new IndexOutOfRangeException();
            var dox = new Document
            {
                DocumentType = dt.DocTypeName
            };
            _parcel.Document.Add(dox);
            p2.Document.Add(dox);

            var touched = await _Ex.Update(_inmates, new Agent { AgentId = Guid.NewGuid(), AgentName = "BUGGER" }, "notes", DateTimeOffset.Now, dox, null);
            Assert.True(touched);

            // should always have a audit history
            _history.Verify(c => c.Add(It.IsAny<StatusActivity>()), Times.Exactly(2));
            _ctx.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Related_Rugged_Parcels_Update_With_DocType()
        {
            _parcel.Parcel_Status = _context.Parcel_Status.Single(sx => sx.Code == "No_Activity");
            _parcel.ParcelStatusCode = _parcel.Parcel_Status.Code;

            var s = _context.Parcel_Status.Single(sx => sx.Code == "Closing");
            var p2 = new Parcel
            {
                ParcelStatusCode = s.Code,
                Parcel_Status = s
            };
            _inmates.Add(p2);

            var dt = _docTypes.AllTypes.First(dx => dx.DisplayOrder == 11) ?? throw new IndexOutOfRangeException();
            var dox = new Document
            {
                DocumentType = dt.DocTypeName
            };
            _parcel.Document.Add(dox);
            p2.Document.Add(dox);

            var touched = await _Ex.Update(_inmates, new Agent { AgentId = Guid.NewGuid(), AgentName = "BUGGER" }, "notes", DateTimeOffset.Now, dox, null);
            Assert.True(touched);

            // should always have a audit history
            _history.Verify(c => c.Add(It.IsAny<StatusActivity>()), Times.Exactly(2));
            _ctx.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Simple_Update_With_Log()
        {
            _parcel.Parcel_Status = _context.Parcel_Status.Single(sx => sx.Code == "No_Activity");
            _parcel.ParcelStatusCode = _parcel.Parcel_Status.Code;

            var log = new ContactLog
            {
                ProjectPhase = "Not_Used"
            };
            _parcel.ContactLog.Add(log);

            var touched = await _Ex.Update(_inmates, new Agent { AgentId = Guid.NewGuid(), AgentName = "BUGGER" }, "notes", DateTimeOffset.Now, null, log);
            Assert.True(touched);

            _history.Verify(c => c.Add(It.IsAny<StatusActivity>()), Times.Once());
            _ctx.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineAutoData(0,11)]  // normal
        [InlineAutoData(12, 11)]    // skip
        [InlineAutoData(14, 11)]    // skip at end
        [InlineAutoData(15)]
        [InlineAutoData(21)]
        public async Task Update_Non_Sequential_Stage_Changes(int before, [Range(11,16)]int after, string notes)
        {
            _log.WriteLine($"{before} -> {after}");

            _parcel.Parcel_Status = _context.Parcel_Status.Single(sx => sx.DisplayOrder == before);
            _parcel.ParcelStatusCode = _parcel.Parcel_Status.Code;

            var dt = _docTypes.AllTypes.First(dx => dx.DisplayOrder == after) ?? throw new IndexOutOfRangeException();
            var dox = new Document
            {
                DocumentType = dt.DocTypeName
            };
            _parcel.Document.Add(dox);

            var touched = await _Ex.Update(_inmates, new Agent { AgentId = Guid.NewGuid(), AgentName = "BUGGER" }, notes, DateTimeOffset.Now, dox, null);
            Assert.Equal(after > before, touched);

            // should always have a audit history
            _history.Verify(c => c.Add(It.IsAny<StatusActivity>()), Times.Once());
            _ctx.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Not_Triggering_DocType()
        {
            _parcel.Parcel_Status = _context.Parcel_Status.Single(sx => sx.DisplayOrder == 0);
            _parcel.ParcelStatusCode = _parcel.Parcel_Status.Code;

            var dt = _docTypes.AllTypes.First(dx => dx.DisplayOrder == 999) ?? throw new IndexOutOfRangeException();
            var dox = new Document
            {
                DocumentType = dt.DocTypeName
            };
            _parcel.Document.Add(dox);

            var touched = await _Ex.Update(_inmates, new Agent { AgentId = Guid.NewGuid(), AgentName = "BUGGER" }, string.Empty, DateTimeOffset.Now, dox, null);
            Assert.False(touched);

            _history.Verify(c => c.Add(It.IsAny<StatusActivity>()), Times.Never());
            _ctx.Verify(c => c.SaveChangesAsync(), Times.Never());
        }

        #region helper
        [Obsolete]
        async Task<bool> Check(bool isDirty = true, Parcel_Status c = default)
        {
            var ( dirty, proposed ) = await _logic.DoUpdate(_parcel);
            Assert.Equal(isDirty, dirty);
            Assert.Equal(c, proposed);

            _basic.AcquisitionStatus = proposed.Code;

            var touched = await _basic.Apply();
            Assert.True(touched >= 0);

            return touched >= 0;
        }
        #endregion

        #region fixture
        public UpdaterTests(ITestOutputHelper h)
        {
            _log = h;

            _context = new ROWM_Context();

            _ctx = new Mock<ROWM_Context>();
            _ctx.Setup(x => x.Parcel_Status).Returns(_context.Parcel_Status);
            _ctx.Setup(x => x.Roe_Status).Returns(_context.Roe_Status);
            _ctx.Setup(x => x.Landowner_Score).Returns(_context.Landowner_Score);
            _ctx.Setup(x => x.Contact_Purpose).Returns(_context.Contact_Purpose);

            _history = new Mock<DbSet<StatusActivity>>();
            _ctx.Setup(x => x.Activities).Returns(_history.Object);

            _status = new Mock<ParcelStatusHelper>(_ctx.Object);
            _repo = new Mock<OwnerRepository>(_ctx.Object);

            _ags = new Mock<IFeatureUpdate>();

            _docTypes = new DocTypes(_context);
            _purposes = _context.Contact_Purpose.ToArray();

            _parcel = new Parcel();

            _inmates = new List<Parcel> { _parcel };

            _basic = new UpdateParcelStatus( 
                parcels: _inmates, 
                agent: new Agent { AgentId = Guid.NewGuid(), AgentName = "BUGGER" }, 
                context: _ctx.Object, 
                repository: _repo.Object, 
                featureUpdate: _ags.Object, 
                h: _status.Object) ;

            _logic = new UpdateParcelStatus2( p: _repo.Object, dt: _docTypes );

            _Ex = new UpdateParcelStatus_Ex(_ctx.Object, _repo.Object, _ags.Object, _docTypes);
        }

        public void Dispose()
        {
        }

        //readonly string CONN = "data source=tcp:wharton-dev-rowm.database.windows.net;initial catalog=rowm;persist security info=True;user id=rowm_app;password=SbhrDX6Cq5VPcR9z;MultipleActiveResultSets=True;App=EntityFramework";

        readonly ROWM_Context _context;
        readonly Mock<ROWM_Context> _ctx;
        readonly Mock<DbSet<StatusActivity>> _history;

        readonly Mock<OwnerRepository> _repo;
        readonly Mock<IFeatureUpdate> _ags;
        readonly Mock<ParcelStatusHelper> _status;

        readonly IEnumerable<Contact_Purpose> _purposes;
        readonly DocTypes _docTypes;

        #endregion
    }
}
