using System.Data.Entity;
using Entity;

namespace DataSync
{
    public class DatabaseContext<TEntity> : DbContext where TEntity : BaseEntity
    {
        public DatabaseContext() : base("OracleDbContext")
        {
            Configuration.LazyLoadingEnabled = Settings.LazyLoadingEnabled;
        }

        public DbSet<TEntity> Entities { get; set; }
    }
}