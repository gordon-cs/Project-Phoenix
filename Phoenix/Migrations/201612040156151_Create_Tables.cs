namespace Phoenix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Tables : DbMigration
    {
        public override void Up()
        {

              
            
            CreateTable(
                "dbo.Damage",
                c => new
                    {
                        DamageID = c.Int(nullable: false, identity: true),
                        DamageDescription = c.String(),
                        Fine = c.Int(nullable: false),
                        RCIComponentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DamageID)
                .ForeignKey("dbo.RCIComponent", t => t.RCIComponentID, cascadeDelete: true)
                .Index(t => t.RCIComponentID);
            
            CreateTable(
                "dbo.RCIComponent",
                c => new
                    {
                        RCIComponentID = c.Int(nullable: false, identity: true),
                        RCIComponentName = c.String(),
                        RoomRCIID = c.Int(),
                        ResidentRCIID = c.Int(),
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
                        ResidentRCIID = c.Int(nullable: false, identity: true),
                        SessionCode = c.String(),
                        RoomRCIID = c.Int(nullable: false),
                        ResidentAccountID = c.String(),
                    })
                .PrimaryKey(t => t.ResidentRCIID)
                .ForeignKey("dbo.RoomRCI", t => t.RoomRCIID, cascadeDelete: true)
                .Index(t => t.RoomRCIID);
            
            CreateTable(
                "dbo.RoomRCI",
                c => new
                    {
                        RoomRCIID = c.Int(nullable: false, identity: true),
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
            
            
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResidentAdvisorAccount", "ResidentAccountID", "dbo.ResidentAccount");
            DropForeignKey("dbo.Damage", "RCIComponentID", "dbo.RCIComponent");
            DropForeignKey("dbo.RCIComponent", "RoomRCIID", "dbo.RoomRCI");
            DropForeignKey("dbo.RCIComponent", "ResidentRCIID", "dbo.ResidentRCI");
            DropForeignKey("dbo.RoomRCI", "RoomID", "dbo.Room");
            DropForeignKey("dbo.ResidentRCI", "RoomRCIID", "dbo.RoomRCI");
            DropIndex("dbo.ResidentAdvisorAccount", new[] { "ResidentAccountID" });
            DropIndex("dbo.RoomRCI", new[] { "RoomID" });
            DropIndex("dbo.ResidentRCI", new[] { "RoomRCIID" });
            DropIndex("dbo.RCIComponent", new[] { "ResidentRCIID" });
            DropIndex("dbo.RCIComponent", new[] { "RoomRCIID" });
            DropIndex("dbo.Damage", new[] { "RCIComponentID" });
            DropTable("dbo.ResidentAdvisorAccount");
            DropTable("dbo.ResidentAccount");
            DropTable("dbo.Room");
            DropTable("dbo.RoomRCI");
            DropTable("dbo.ResidentRCI");
            DropTable("dbo.RCIComponent");
            DropTable("dbo.Damage");
        }
    }
}
