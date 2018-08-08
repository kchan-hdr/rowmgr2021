namespace Tectra
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model13")
        {
        }

        public virtual DbSet<b2h2_Contacts> b2h2_Contacts { get; set; }
        public virtual DbSet<b2h2_ContactsDocs> b2h2_ContactsDocs { get; set; }
        public virtual DbSet<b2h2_ParcelDocs> b2h2_ParcelDocs { get; set; }
        public virtual DbSet<junk> junks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
