using FluentAssertions;
using ROWM.Dal;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ROWM_Net50.Dal.Test
{
    public class RepositoryTests : IClassFixture<DbFixture>
    {
        StatisticsRepository _repo;

        readonly ITestOutputHelper _log;
        readonly DbFixture _db;
        public RepositoryTests(ITestOutputHelper h, DbFixture d) => (_log, _db) = (h, d);


        [Fact, Trait("Category", "DAL 5 Repo")]
        public async Task Get_Statistics()
        {
            var repo = new StatisticsRepository(_db.Context);
            var s = await repo.Snapshot();

            s.Should()
                .NotBeNull();

            s.nOwners.Should().BeGreaterThan(0);
            s.nParcels.Should().BeGreaterThan(s.nOwners);
        }

        [Fact, Trait("Category", "DAL 5 Repo")]
        public async Task Get_Parcel_Statistics()
        {
            var repo = new StatisticsRepository(_db.Context);
            var s = await repo.SnapshotParcelStatus();

            s.Should().NotBeNullOrEmpty();

            foreach( var cat in s)
            {
                string.IsNullOrWhiteSpace(cat.Title).Should().BeFalse();
                cat.Count.Should().BeGreaterOrEqualTo(0);
                _log.WriteLine($"{cat.Title} {cat.Caption} {cat.Count}");
            }
        }

        [Fact, Trait("Category", "DAL 5 Repo")]
        public async Task Get_ROE_Statistics()
        {
            var repo = new StatisticsRepository(_db.Context);
            var s = await repo.SnapshotRoeStatus();

            s.Should().NotBeNullOrEmpty();

            foreach (var cat in s)
            {
                string.IsNullOrWhiteSpace(cat.Title).Should().BeFalse();
                cat.Count.Should().BeGreaterOrEqualTo(0);
                _log.WriteLine($"{cat.Title} {cat.Caption} {cat.Count}");
            }
        }
    }
}
