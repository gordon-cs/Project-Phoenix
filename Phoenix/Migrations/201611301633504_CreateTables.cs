namespace Phoenix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResidentAccount",
                c => new
                    {
                        ResidentAccountID = c.String(nullable: false, maxLength: 128),
                        Year = c.String(),
                    })
                .PrimaryKey(t => t.ResidentAccountID);
            
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
            DropIndex("dbo.ResidentAdvisorAccount", new[] { "ResidentAccountID" });
            DropTable("dbo.ResidentAdvisorAccount");
            DropTable("dbo.ResidentAccount");
        }
    }
}
