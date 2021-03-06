USE [RCITrain]
GO
/****** Object:  View [dbo].[RoomAssign]    Script Date: 6/26/2018 9:34:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RoomAssign]
AS
SELECT        ISNULL(APPID, - 1) AS APPID, SESS_CDE, BLDG_LOC_CDE, BLDG_CDE, ROOM_CDE, ROOM_SLOT_NUM, ID_NUM, ROOM_TYPE, ROOM_ASSIGN_STS, ASSIGN_DTE, APPROWVERSION, USER_NAME, 
                         JOB_NAME, JOB_TIME
FROM            TmsEPrd.dbo.GORD_RCI_ROOM_ASSIGN
UNION
SELECT        '100000' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'WIL' AS BLDG_CDE, '217' AS ROOM_CDE, '1' AS ROOM_SLOT_NUM, '999999097' AS ID_NUM, 'DO' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME
UNION
SELECT        '100001' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'TAV' AS BLDG_CDE, '109B' AS ROOM_CDE, '1' AS ROOM_SLOT_NUM, '50169203' AS ID_NUM, 'AP' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME
UNION
SELECT        '100002' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'TAV' AS BLDG_CDE, '109C' AS ROOM_CDE, '1' AS ROOM_SLOT_NUM, '50180469' AS ID_NUM, 'AP' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME
UNION
SELECT        '100004' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'WIL' AS BLDG_CDE, '403' AS ROOM_CDE, '1' AS ROOM_SLOT_NUM, '999999089' AS ID_NUM, 'DO' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME
UNION
SELECT        '100005' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'TAV' AS BLDG_CDE, '999A' AS ROOM_CDE, '1' AS ROOM_SLOT_NUM, '999999090' AS ID_NUM, 'AP' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME
UNION
SELECT        '100006' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'TAV' AS BLDG_CDE, '999A' AS ROOM_CDE, '2' AS ROOM_SLOT_NUM, '999999091' AS ID_NUM, 'AP' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME
UNION
SELECT        '100007' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'TAV' AS BLDG_CDE, '999B' AS ROOM_CDE, '1' AS ROOM_SLOT_NUM, '999999092' AS ID_NUM, 'AP' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME
UNION
SELECT        '100008' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'TAV' AS BLDG_CDE, '999B' AS ROOM_CDE, '2' AS ROOM_SLOT_NUM, '999999093' AS ID_NUM, 'AP' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME
UNION
SELECT        '100009' AS APPID, '201805' AS SESS_CDE, 'FWLR' AS BLDG_LOC_CDE, 'TAV' AS BLDG_CDE, '1000A' AS ROOM_CDE, '1' AS ROOM_SLOT_NUM, '999999094' AS ID_NUM, 'AP' AS ROOM_TYPE, 
                         'A' AS ROOM_ASSIGN_STS, CONVERT(datetime, 'Oct 23 2016 11:00AM') AS ASSIGN_DTE, 0x1110101010101 AS APPROWVERSION, 'TEST' AS [USER_NAME], 'TEST' AS JOB_NAME, CONVERT(datetime, 
                         'Oct 23 2016 11:00AM') AS JOB_TIME

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[21] 4[13] 2[48] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'RoomAssign'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'RoomAssign'
GO
