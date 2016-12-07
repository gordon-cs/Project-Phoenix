namespace Phoenix.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

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

            // Add RoomRCI
            context.RoomRCI.AddOrUpdate(
                p => p.RoomRCIID,
                new RoomRCI { RoomID = "2652", SessionCode = "Spring" }
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
                new RCIComponent { RCIComponentName = "Shelves", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Window", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Door", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Ceiling", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );
            context.RCIComponent.AddOrUpdate(
                p => p.RCIComponentID,
                new RCIComponent { RCIComponentName = "Wardrobe", RoomRCIID = roomRCIID, ResidentRCIID = residentRCIID }
                );

            context.SaveChanges();

            // Create Damages

            context.Damage.AddOrUpdate(
                p => p.DamageID,
                new Damage { DamageDescription = "Small scratch near leg", RCIComponentID = 2 },
                new Damage { DamageDescription = "Upholstery torn", RCIComponentID = 2 },
                new Damage { DamageDescription = "Watermelon-sized stain near front", RCIComponentID = 4 },
                new Damage { DamageDescription = "Tear near bed", RCIComponentID = 4 },
                new Damage { DamageDescription = "leads to narnia", RCIComponentID = 9 },
                new Damage { DamageDescription = "creaky left door", RCIComponentID = 9 },
                new Damage { DamageDescription = "mountain range-shaped wood chips", RCIComponentID = 9 },
                new Damage { DamageDescription = "intricate mural of the history of twine painted on right wall", RCIComponentID = 3 },
                new Damage { DamageDescription = "A resident ran off with this when I came in", RCIComponentID = 5 },
                new Damage { DamageDescription = "Silhouette of someone who was defenestrated", RCIComponentID = 6 }
                );
            context.SaveChanges();
        }
    }
}
