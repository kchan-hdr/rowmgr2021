using FluentAssertions;
using ROWM.Dal.Repository;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ROWM_Net50.Dal.Test
{
    public class ParcelListingTests : IClassFixture<DbFixture>
    {
        readonly ITestOutputHelper _log;
        readonly DbFixture _db;
        public ParcelListingTests(ITestOutputHelper h, DbFixture d) => (_log, _db) = (h, d);

        [Theory, Trait("Category", "Status Hierarchy")]
        [InlineData("Offer", false)]
        [InlineData("Survey", false)]
        [InlineData("Condementation", false)]
        public async Task Simple_List(string milestone, bool hasRow)
        {
            var repo = new ParcelStatusRepository(_db.Context, null);
            var list = await repo.GetParcelList(milestone);

            Assert.Equal(hasRow, list.Any());
        }

        [Theory, Trait("Category", "Status Hierarchy")]
        [InlineData("Doesn't exists")]
        public async Task Bad_key(string milestone)
        {
            var repo = new ParcelStatusRepository(_db.Context, null);
            var list = await repo.GetParcelList(milestone);

            list.Should().BeEmpty();
        }
    }
}
