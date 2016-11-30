namespace Phoenix.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using Phoenix.Models.PreExistingViews;
    //using Phoenix.Migrations;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class RCIContext : DbContext
    {
        public RCIContext()
            : base("name=RCIContext")
        {
        }

        /* Pre-existing Views */
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<CurrentRA> CurrentRA { get; set; }
        public virtual DbSet<CurrentRD> CurrentRD { get; set; }
        public virtual DbSet<RoomAssign> RoomAssign { get; set; }
        public virtual DbSet<RoomChangeHistory> RoomChangeHistory { get; set; }
        public virtual DbSet<Session> Session { get; set; }


        /* Tables we create */
        public virtual DbSet<ResidentAccount> ResidentAccount { get; set; }
        public virtual DbSet<ResidentAdvisorAccount> ResidentAdvisorAccount { get; set; }
        public virtual DbSet<ResidentRCI> ResidentRCI { get; set; }
        public virtual DbSet<RoomRCI> RoomRCI { get; set; }
        public virtual DbSet<RCIComponent> RCIComponent { get; set; }
        public virtual DbSet<Damage> Damage { get; set; }
        public virtual DbSet<Room> Room { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.SetInitializer<RCIContext>(new MigrateDatabaseToLatestVersion<RCIContext, Configuration>());
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);        
        }
    }
}
