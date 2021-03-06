USE [RCITrain]
GO
/****** Object:  StoredProcedure [dbo].[FindMissingRcis]    Script Date: 6/26/2018 9:34:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Eze Anyanwu
-- Create date: 2/22/2017
-- Description:	Finds the roomassign records for which rcis need to be generated in a particular building
-- =============================================
CREATE PROCEDURE [dbo].[FindMissingRcis] 
	-- Add the parameters for the stored procedure here
	@building nvarchar(50), 
	@currentSession nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	select APPID, 
		SESS_CDE, 
		BLDG_LOC_CDE, 
		BLDG_CDE, 
		ROOM_CDE, 
		ROOM_SLOT_NUM, 
		ID_NUM, 
		ROOM_TYPE, 
		ROOM_ASSIGN_STS, 
		ASSIGN_DTE, 
		APPROWVERSION, 
		USER_NAME, 
		JOB_NAME, 
		JOB_TIME 
	from RoomAssign as rm
	left join Rci as rci
		on rm.ID_NUM = rci.GordonID
		and rm.BLDG_CDE = rci.BuildingCode
		and rm.ROOM_CDE = rci.RoomNumber
		and rm.ASSIGN_DTE < rci.CreationDate
	where
		rm.BLDG_CDE = @building 
		and rm.SESS_CDE = @currentSession
		and rm.ID_NUM is not null
		and rm.ASSIGN_DTE is not null
		and rci.RciID is null 


END

GO
