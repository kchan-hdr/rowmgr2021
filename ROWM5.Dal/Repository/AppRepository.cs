using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ROWM.Dal
{
    public class AppRepository
    {
        readonly ROWM_Context _Context;
        public AppRepository(ROWM_Context c) => this._Context = c;

        public IEnumerable<Map> GetLayers() => this._Context.Maps.AsNoTracking().ToList();
    }
}
