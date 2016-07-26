using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using UkadTask.Domain;
using UkadTask.Repository.Mapping;

namespace UkadTask.Repository
{
    public partial class EFContext : DbContext
    {
        static EFContext()
        {
            Database.SetInitializer<EFContext>(null);
        }

        public EFContext()
            : base("Name=UkadTaskContext")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<History> History { get; set; }
        public DbSet<Host> Hosts { get; set; }
        public DbSet<Page> Pages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new HistoryMap());
            modelBuilder.Configurations.Add(new HostMap());
            modelBuilder.Configurations.Add(new PageMap());
        }
    }
}
