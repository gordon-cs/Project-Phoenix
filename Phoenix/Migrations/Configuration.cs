namespace Phoenix.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;


    using Phoenix.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Phoenix.Models.RCIContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Phoenix.Models.RCIContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // Add Room
            context.Room.AddOrUpdate(
                p => p.RoomID,
                new Room { RoomID = "WIL-217", Capacity = 1 }
                );
            // Add RoomRCI
            context.RoomRCI.AddOrUpdate(
                p => p.RoomRCIID,
                new RoomRCI { RoomID = "WIL-217", SessionCode = "Spring" }
                );
            context.SaveChanges();

            // Fetch the RoomRCI id
            var roomRCI = context.RoomRCI.FirstOrDefault();
            var roomRCIID = roomRCI.RoomRCIID;

            // Create the residentRCI
            context.ResidentRCI.AddOrUpdate(
               p => p.ResidentRCIID,
               new ResidentRCI { RoomRCIID = roomRCIID, SessionCode = "Spring", ResidentAccountID = "50153295" }
               );

            context.SaveChanges();

            // Fetch the residentRCI id
            var residentRCI = context.ResidentRCI.FirstOrDefault();
            var residentRCIID = residentRCI.ResidentRCIID;

            // Create RCI components
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Desk", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Chair", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Wall", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Carpet", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Shelves above desk", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );

            context.SaveChanges();

            // Create Damages

            context.Damage.AddOrUpdate(
                p => p.DamageID,
                new Damage { DamageDescription = "Small scratch near leg", RCIComponentID = 2 },
                new Damage { DamageDescription = "Upholstery torn", RCIComponentID = 2 },
                new Damage { DamageDescription = "Watermelon-sized stain near front", RCIComponentID = 4 },
                new Damage { DamageDescription = "Tear near bed", RCIComponentID = 4 }
                );
            context.SaveChanges();
        }
    }
}
