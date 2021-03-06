USE [RCITrain]
GO
/****** Object:  View [dbo].[Room]    Script Date: 6/26/2018 9:34:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Room]
AS
SELECT        ISNULL(LOC_CDE, '') AS LOC_CDE, ISNULL(BLDG_CDE, '') AS BLDG_CDE, BUILDING_DESC, ISNULL(ROOM_CDE, '') AS ROOM_CDE, ROOM_DESC, ROOM_TYPE, ROOM_TYPE_DESC, MAX_CAPACITY, 
                         ROOM_GENDER, RM_WHICH_FLOOR
FROM            TmsEPrd.dbo.GORD_RCI_ROOM_MASTER
WHERE        (ROOM_TYPE IN ('SI', 'DO', 'TR', 'QU', 'SU', 'AP', 'LV')) OR
                         (ROOM_TYPE = 'KT') AND (BLDG_CDE = 'FER')
UNION
SELECT        'FWLR' AS LOC_CDE, 'TAV' AS BLDG_CDE, 'Tavilla Hall' AS BUILDING_DESC, '999' AS 'ROOM_CDE', 'FWLR-TAV999 AR-Apartment' AS ROOM_DESC, 'AP' AS ROOM_TYPE, 'AR-Apartment' AS ROOM_TYPE_DESC, 
                         '0' AS MAX_CAPACITY, NULL AS ROOM_GENDER, '9' AS RM_WHICH_FLOOR
UNION
SELECT         'FWLR' as LOC_CDE, 'TAV' as BLDG_CDE, 'Tavilla Hall' as BUILDING_DESC, '1000' as 'ROOM_CDE', 'FWLR-TAV1000 AR-Apartment' as ROOM_DESC, 'AP' as ROOM_TYPE, 'AR-Apartment' as ROOM_TYPE_DESC,
                           '0' as MAX_CAPACITY, NULL as ROOM_GENDER,  '9' as RM_WHICH_FLOOR

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Room'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Room'
GO
