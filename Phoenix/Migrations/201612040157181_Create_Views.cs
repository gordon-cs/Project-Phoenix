namespace Phoenix.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Views : DbMigration
    {
        public override void Up()
        {

            // Create Account View
            Sql("EXEC ('CREATE VIEW [dbo].[Account] AS SELECT ISNULL(ID_NUM, - 1) AS ID_NUM, firstname, lastname, email, AD_Username, account_type, CASE WHEN ID_NUM = ''999999097'' THEN 1 ELSE Private END AS Private FROM webSQL.dbo.view_rci')");

            // Create Session View
            Sql("EXEC ('CREATE VIEW [dbo].[Session] AS SELECT ISNULL(SESS_CDE, - 1) AS SESS_CDE, SESS_DESC, SESS_BEGN_DTE, SESS_END_DTE FROM TmsEPrd.dbo.GORD_RCI_CM_SESSION_MSTR')");

            // Create CurrentRA View
            Sql("EXEC ('CREATE VIEW [dbo].[CurrentRA] AS SELECT Dorm, AD_Username, ISNULL(ID_NUM, - 1) AS ID_NUM FROM Identify.dbo.view_rci_current_ras')");

            // Create CurrentRD View
            Sql("EXEC ('CREATE VIEW [dbo].[CurrentRD] AS SELECT ISNULL(ID_NUM, - 1) AS ID_NUM, firstname, lastname, email, Job_Title, Job_Title_Hall FROM webSQL.dbo.view_rci_current_rds')");

            // Create RoomAssign View
            Sql("EXEC ('CREATE VIEW [dbo].[RoomAssign] AS SELECT ISNULL(APPID, - 1) AS APPID, SESS_CDE, BLDG_LOC_CDE, BLDG_CDE, ROOM_CDE, ROOM_SLOT_NUM, ID_NUM, ROOM_TYPE, ROOM_ASSIGN_STS, ASSIGN_DTE, APPROWVERSION, USER_NAME, JOB_NAME, JOB_TIME FROM TmsEPrd.dbo.GORD_RCI_ROOM_ASSIGN')");

            // Create RoomChangeHistory View
            Sql("EXEC ('CREATE VIEW [dbo].[RoomChangeHistory] AS SELECT SESS_CDE, ID_NUM, ROOM_CHANGE_DTE, OLD_BLDG_LOC_CDE, OLD_BLDG_CDE, OLD_ROOM_CDE, NEW_BLDG_LOC_CDE, NEW_BLDG_CDE, NEW_ROOM_CDE, ROOM_CHANGE_REASON, ROOM_CHANGE_COMMENT, USER_NAME, JOB_NAME, JOB_TIME FROM TmsEPrd.dbo.GORD_RCI_ROOM_CHANGE_HIST')");

        }

        public override void Down()
        {
            // Drop Account View
            Sql("EXEC ('DROP VIEW Account')");

            // Drop Session View
            Sql("EXEC ('DROP VIEW Session')");

            // Drop CurrentRA View
            Sql("EXEC ('DROP VIEW CurrentRA')");

            // Drop CurrentRD View
            Sql("EXEC ('DROP VIEW CurrentRD')");

            // Drop RoomAssign View
            Sql("EXEC ('DROP VIEW RoomAssign')");

            // Drop RoomChangeHistory View
            Sql("EXEC ('DROP VIEW RoomChangeHistory')");
        }
    }
}
