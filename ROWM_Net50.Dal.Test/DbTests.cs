using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ROWM_Net50.Dal.Test
{
    public class DbTests : IClassFixture<DbFixture>
    {
        readonly ITestOutputHelper _log;
        readonly DbFixture _db;
        public DbTests(ITestOutputHelper h, DbFixture d) => (_log, _db) = (h, d);   

        [Fact, Trait("Category", "EF Core 5")]
        public async Task Simple_Connection()
        {
            var c = _db.Context;
            var cnt = await c.Owner.CountAsync();

            cnt.Should().BeGreaterThan(0);
        }

        [Fact, Trait("Category", "EF Core 5")]
        public async Task EF_Core_Many_To_Many()
        {
            var c = _db.Context;
            var myOwners = c.Owner
                .Include(o => o.Ownership)
                .Include(o => o.ContactInfo);
            _log.WriteLine(myOwners.ToQueryString());

            foreach(var o in myOwners)
            {
                o.Ownership.Count
                    .Should()
                    .BeGreaterThan(0);

                _log.WriteLine($"{o.PartyName} {o.Ownership.Count}");
            }

            var aglog = from l in c.ContactLog
                        join a in c.Agent on l.ContactAgentId equals a.AgentId into ag
                        from a in ag.DefaultIfEmpty() 
                        select new { Agent = a, Log = l };

            _log.WriteLine(aglog.ToQueryString());

            var agent = await aglog.FirstAsync();

            agent.Should().NotBeNull();
            agent.Log.Should().BeOfType(typeof(ROWM.Dal.ContactLog));


            _log.WriteLine(agent.Log.Title);
            
        }
    }
}
