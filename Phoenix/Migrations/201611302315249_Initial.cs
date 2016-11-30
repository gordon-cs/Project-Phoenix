namespace Phoenix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {            
            
            CreateTable(
                "dbo.Damage",
                c => new
                    {
                        DamageID = c.String(nullable: false, maxLength: 128),
                        DamageDescription = c.String(),
                        Fine = c.Int(nullable: false),
                        RCIComponentID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.DamageID)
                .ForeignKey("dbo.RCIComponent", t => t.RCIComponentID)
                .Index(t => t.RCIComponentID);
            
            CreateTable(
                "dbo.RCIComponent",
                c => new
                    {
                        RCIComponentID = c.String(nullable: false, maxLength: 128),
                        RCIComponentName = c.String(),
                        RoomRCIID = c.String(maxLength: 128),
                        ResidentRCIID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RCIComponentID)
                .ForeignKey("dbo.ResidentRCI", t => t.ResidentRCIID)
                .ForeignKey("dbo.RoomRCI", t => t.RoomRCIID)
                .Index(t => t.RoomRCIID)
                .Index(t => t.ResidentRCIID);
            
            CreateTable(
                "dbo.ResidentRCI",
                c => new
                    {
                        ResidentRCIID = c.String(nullable: false, maxLength: 128),
                        SessionCode = c.String(),
                        ResidentAccountID = c.String(maxLength: 128),
                        RoomRCI_RoomRCIID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ResidentRCIID)
                .ForeignKey("dbo.ResidentAccount", t => t.ResidentAccountID)
                .ForeignKey("dbo.RoomRCI", t => t.RoomRCI_RoomRCIID)
                .Index(t => t.ResidentAccountID)
                .Index(t => t.RoomRCI_RoomRCIID);
            
            CreateTable(
                "dbo.ResidentAccount",
                c => new
                    {
                        ResidentAccountID = c.String(nullable: false, maxLength: 128),
                        Year = c.String(),
                    })
                .PrimaryKey(t => t.ResidentAccountID);
            
            CreateTable(
                "dbo.RoomRCI",
                c => new
                    {
                        RoomRCIID = c.String(nullable: false, maxLength: 128),
                        SessionCode = c.String(),
                        RoomID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RoomRCIID)
                .ForeignKey("dbo.Room", t => t.RoomID)
                .Index(t => t.RoomID);
            
            CreateTable(
                "dbo.Room",
                c => new
                    {
                        RoomID = c.String(nullable: false, maxLength: 128),
                        Capacity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoomID);
            
            CreateTable(
                "dbo.ResidentAdvisorAccount",
                c => new
                    {
                        ResidentAccountID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ResidentAccountID)
                .ForeignKey("dbo.ResidentAccount", t => t.ResidentAccountID)
                .Index(t => t.ResidentAccountID);            
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResidentAdvisorAccount", "ResidentAccountID", "dbo.ResidentAccount");
            DropForeignKey("dbo.Damage", "RCIComponentID", "dbo.RCIComponent");
            DropForeignKey("dbo.RCIComponent", "RoomRCIID", "dbo.RoomRCI");
            DropForeignKey("dbo.RoomRCI", "RoomID", "dbo.Room");
            DropForeignKey("dbo.ResidentRCI", "RoomRCI_RoomRCIID", "dbo.RoomRCI");
            DropForeignKey("dbo.RCIComponent", "ResidentRCIID", "dbo.ResidentRCI");
            DropForeignKey("dbo.ResidentRCI", "ResidentAccountID", "dbo.ResidentAccount");
            DropIndex("dbo.ResidentAdvisorAccount", new[] { "ResidentAccountID" });
            DropIndex("dbo.RoomRCI", new[] { "RoomID" });
            DropIndex("dbo.ResidentRCI", new[] { "RoomRCI_RoomRCIID" });
            DropIndex("dbo.ResidentRCI", new[] { "ResidentAccountID" });
            DropIndex("dbo.RCIComponent", new[] { "ResidentRCIID" });
            DropIndex("dbo.RCIComponent", new[] { "RoomRCIID" });
            DropIndex("dbo.Damage", new[] { "RCIComponentID" });
            DropTable("dbo.ResidentAdvisorAccount");
            DropTable("dbo.Room");
            DropTable("dbo.RoomRCI");
            DropTable("dbo.ResidentAccount");
            DropTable("dbo.ResidentRCI");
            DropTable("dbo.RCIComponent");
            DropTable("dbo.Damage");

        }
    }
}
