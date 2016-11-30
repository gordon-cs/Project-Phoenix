namespace Phoenix.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using Phoenix.Models.PreExistingViews;

    public partial class RCIContext : DbContext
    {
        public RCIContext()
            : base("name=RCIContext")
        {
        }

        /*  Pre-Existing Views. No Tables should be created for theses */
        public virtual DbSet<ACCOUNT> ACCOUNT { get; set; }
        public virtual DbSet<CM_SESSION_MSTR> CM_SESSION_MSTR { get; set; }
        public virtual DbSet<CURRENT_RAS> CURRENT_RAS { get; set; }
        public virtual DbSet<CURRENT_RDS> CURRENT_RDS { get; set; }
        public virtual DbSet<ROOM_ASSIGN> ROOM_ASSIGN { get; set; }
        public virtual DbSet<ROOM_MASTER> ROOM_MASTER { get; set; }


        /* Tables we created */
        public DbSet <>


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
     
        }
    }
}
