using Hangfire.Database.Classes;
using Hangfire.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Hangfire.Database
{
    public class HangfireContextOptions
    {
        public string ConnectionString { get; set; }
        public Func<IHttpContextAccessor, string> UsernameGetterFunction { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        readonly HangfireContextOptions options;
        readonly IHttpContextAccessor httpContextAccessor;
        private readonly string _connectionString;

        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(HangfireContextOptions options, IHttpContextAccessor httpContextAccessor)
        {
            this.options = options;
            this.httpContextAccessor = httpContextAccessor;
        }

        public ApplicationDbContext(HangfireContextOptions hangfireOptions, IHttpContextAccessor httpContextAccessor, DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.options = hangfireOptions;
            this.httpContextAccessor = httpContextAccessor;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(options.ConnectionString, builder =>
                {
                });
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocZCustIDLast>().HasNoKey();
            modelBuilder.Entity<LocZCustIDLastRelation>().HasNoKey();
        }

        // Add Entities
        public DbSet<TblLogTransection> LogTransections { get; set; }
        public DbSet<TblCrypt> Crypts { get; set; }
        public DbSet<TempDwhPdpa> TempDwhPdpas { get; set; }
        public DbSet<TdrPdpaDelLog> TdrPdpaDelLogs { get; set; }
        public DbSet<TdrMst> TdrMsts { get; set; }
        public DbSet<TblAccount> TblAccounts { get; set; }
        public DbSet<TdrRefaccno> TdrRefaccnos { get; set; }
        public DbSet<TdrScheme> TdrSchemes { get; set; }
        public DbSet<DataDwhDaily> DataDwhDailies { get; set; }
        public DbSet<DataFtpAls> DataFtpAls { get; set; }
        public DbSet<LocZCustIDLast> LocZCustIDLasts { get; set; }
        public DbSet<LocZCustIDLastRelation> LocZCustIDLastRelations { get; set; }
        public DbSet<TblResult> TblResults { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await this.CustomSaveChangesAsync(true, cancellationToken);
        }

        public async Task<int> CustomSaveChangesAsync(bool stampModified, CancellationToken cancellationToken = default)
        {
            if (!this.ChangeTracker.AutoDetectChangesEnabled)
            {
                this.ChangeTracker.DetectChanges();
            }
            UpdateFieldBeforeSave(stampModified);
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected void UpdateFieldBeforeSave(bool stampModified, bool defaultSystem = false)
        {
            DateTime now = DateTime.Now;

            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList())
            {
                if (stampModified)
                {
                    SetProperty(entry, now, defaultSystem);
                }

                var beforeSavedMethod = entry.Entity.GetType().GetMethod("BeforeSaved");

                if (beforeSavedMethod != null)
                    beforeSavedMethod.Invoke(entry.Entity, new Object[] { });
            }
        }

        private void SetProperty(EntityEntry entry, DateTime now, bool defaultSystem)
        {
            SetCreatedProperties(entry, now, defaultSystem);
            SetUpdatedProperties(entry, now, defaultSystem);
        }

        private void SetCreatedProperties(EntityEntry entry, DateTime now, bool defaultSystem)
        {
            var createdDateProp = entry.Entity.GetType().GetProperty("CreatedDate");
            var createdByProp = entry.Entity.GetType().GetProperty("CreatedBy");

            if (entry.State == EntityState.Added)
            {
                SetPropertyValueIfNull(createdDateProp, entry.Entity, now);
                SetCreatedByProperty(createdByProp, entry.Entity, defaultSystem);
            }
        }

        private void SetUpdatedProperties(EntityEntry entry, DateTime now, bool defaultSystem)
        {
            var updatedDateProp = entry.Entity.GetType().GetProperty("UpdatedDate");
            var updatedByProp = entry.Entity.GetType().GetProperty("UpdatedBy");

            if (entry.State != EntityState.Added)
            {
                SetPropertyValue(updatedDateProp, entry.Entity, now);
                SetUpdatedByProperty(updatedByProp, entry.Entity, defaultSystem);
            }
        }
        private void SetCreatedByProperty(PropertyInfo? property, object entity, bool defaultSystem)
        {
            if (property != null)
            {
                property.SetValue(entity, defaultSystem ? "System" : GetCurrentUserName());
            }
        }

        private void SetPropertyValueIfNull(PropertyInfo? property, object entity, DateTime value)
        {
            if (property != null)
            {
                DateTime oldValue = Convert.ToDateTime(property.GetValue(entity));
                if (oldValue == DateTime.MinValue)
                {
                    property.SetValue(entity, value);
                }
            }
        }

        private void SetPropertyValue(PropertyInfo? property, object entity, DateTime value)
        {
            if (property != null)
            {
                property.SetValue(entity, value);
            }
        }

        private void SetUpdatedByProperty(PropertyInfo? property, object entity, bool defaultSystem)
        {
            if (property != null)
            {
                property.SetValue(entity, defaultSystem ? "System" : GetCurrentUserName());
            }
        }

        protected string GetCurrentUserName()
        {
            if (options?.UsernameGetterFunction != null)
                return options.UsernameGetterFunction(this.httpContextAccessor);
            else
                return "";
        }

        public bool PerformMigration(bool forceUpdateDescription = false)
        {
            var pendingMigrationCount = Database.GetPendingMigrations().Count();
            var needMigration = pendingMigrationCount > 0;
            var updateDescription = forceUpdateDescription || needMigration;

            if (needMigration)
            {
                Database.Migrate();
                updateDescription = true;
            }
            if (updateDescription)
            {
                DbDescriptionUpdater<ApplicationDbContext> updater = new DbDescriptionUpdater<ApplicationDbContext>(this);
                try
                {
                    updater.UpdateDatabaseDescriptions();
                }
                catch (Exception)
                {
                    // silent db comment updator
                }
            }
            this.SaveChanges();

            return needMigration;
        }
    }
}

