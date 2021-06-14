using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM_Net50.Dal.Test
{
    public class DbFixture : IDisposable
    {
        public ROWM.Dal.ROWM_Context Context { get; private set; }

        public DbFixture()
        {
            var o = new DbContextOptionsBuilder<ROWM.Dal.ROWM_Context>()
                .UseSqlServer("Server=tcp:atp-rowm-dev.database.windows.net,1433;Initial Catalog=rowm;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Interactive;")
                .LogTo(Console.WriteLine);

            this.Context = new ROWM.Dal.ROWM_Context(o.Options);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.Context != null)
                        this.Context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DbFixture()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
