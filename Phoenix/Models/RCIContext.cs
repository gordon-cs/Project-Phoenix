namespace Phoenix.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RCIContext : DbContext
    {
        public RCIContext()
            : base("name=RCIContext")
        {
        }

        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<CommonAreaRciSignature> CommonAreaRciSignature { get; set; }
        public virtual DbSet<Damage> Damage { get; set; }
        public virtual DbSet<Fine> Fine { get; set; }
        public virtual DbSet<Rci> Rci { get; set; }
        public virtual DbSet<RciComponent> RciComponent { get; set; }
        public virtual DbSet<BuildingAssign> BuildingAssign { get; set; }
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<CurrentRA> CurrentRA { get; set; }
        public virtual DbSet<CurrentRD> CurrentRD { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<RoomAssign> RoomAssign { get; set; }
        public virtual DbSet<RoomChangeHistory> RoomChangeHistory { get; set; }
        public virtual DbSet<Session> Session { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Damage>()
                .Property(e => e.FineAssessed)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Fine>()
                .Property(e => e.FineAmount)
                .HasPrecision(13, 2);

            modelBuilder.Entity<RciComponent>()
                .HasMany(e => e.Fine)
                .WithOptional(e => e.RciComponent)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Account>()
                .Property(e => e.ID_NUM)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.firstname)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.lastname)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.AD_Username)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.account_type)
                .IsUnicode(false);

            modelBuilder.Entity<CurrentRA>()
                .Property(e => e.Dorm)
                .IsUnicode(false);

            modelBuilder.Entity<CurrentRA>()
                .Property(e => e.AD_Username)
                .IsUnicode(false);

            modelBuilder.Entity<CurrentRD>()
                .Property(e => e.ID_NUM)
                .IsUnicode(false);

            modelBuilder.Entity<CurrentRD>()
                .Property(e => e.firstname)
                .IsUnicode(false);

            modelBuilder.Entity<CurrentRD>()
                .Property(e => e.lastname)
                .IsUnicode(false);

            modelBuilder.Entity<CurrentRD>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<CurrentRD>()
                .Property(e => e.Job_Title)
                .IsUnicode(false);

            modelBuilder.Entity<CurrentRD>()
                .Property(e => e.Job_Title_Hall)
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.LOC_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.BLDG_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.BUILDING_DESC)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.ROOM_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.ROOM_DESC)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.ROOM_TYPE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.ROOM_TYPE_DESC)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.ROOM_GENDER)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.SESS_CDE)
                .IsUnicode(false);

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.BLDG_LOC_CDE)
                .IsUnicode(false);

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.BLDG_CDE)
                .IsUnicode(false);

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.ROOM_CDE)
                .IsUnicode(false);

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.ROOM_TYPE)
                .IsUnicode(false);

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.ROOM_ASSIGN_STS)
                .IsUnicode(false);

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.APPROWVERSION)
                .IsFixedLength();

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.USER_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<RoomAssign>()
                .Property(e => e.JOB_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.SESS_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.OLD_BLDG_LOC_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.OLD_BLDG_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.OLD_ROOM_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.NEW_BLDG_LOC_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.NEW_BLDG_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.NEW_ROOM_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.ROOM_CHANGE_REASON)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.ROOM_CHANGE_REASON_DESC)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<RoomChangeHistory>()
                .Property(e => e.ROOM_CHANGE_COMMENT)
                .IsUnicode(false);

            modelBuilder.Entity<Session>()
                .Property(e => e.SESS_CDE)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Session>()
                .Property(e => e.SESS_DESC)
                .IsUnicode(false);
        }
    }
}
