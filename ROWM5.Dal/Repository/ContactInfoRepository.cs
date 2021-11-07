using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

#nullable enable
    
namespace ROWM.Dal
{
    public class ContactInfoRepository
    {
        readonly ROWM_Context _context;
        public ContactInfoRepository(ROWM_Context c) => _context = c;

        public async Task<Organization> FindOrganization(string n) =>
            await _context.Organizations.SingleOrDefaultAsync(ox => ox.Name.Equals(n));

        public async Task<Organization> Get(Guid id) => await _context.Organizations.FindAsync(id);
    }
}
