USE [RCITrain]
GO
/****** Object:  View [dbo].[Account]    Script Date: 6/26/2018 9:34:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Account]
AS
SELECT        ISNULL(ID_NUM, - 1) AS ID_NUM, firstname, lastname, email, AD_Username, account_type, Private
FROM            webSQL.dbo.view_rci
UNION
SELECT        '999999097' AS ID_NUM, '360' AS firstname, 'StudentTest' AS lastname, '360.studenttest@gordon.edu' AS email, '360.studenttest' AS AD_Username, 'STUDENT' AS account_type, 0 Private
UNION
SELECT        '999999098' AS ID_NUM, '360' AS firstname, 'StaffTest' AS lastname, '360.stafftest@gordon.edu' AS email, '360.stafftest' AS AD_Username, 'STAFF' AS account_type, 0 Private
UNION
SELECT        '999999099' AS ID_NUM, '360' AS firstname, 'FacultyTest' AS lastname, '360.facultytest@gordon.edu' AS email, '360.facultytest' AS AD_Username, 'FACULTY' AS account_type, 0 Private
UNION
SELECT        '999999089' AS ID_NUM, 'Rci-Test1' AS firstname, '' AS lastname, 'rci-test1@gordon.edu' AS email, 'Rci-test1' AS AD_Username, 'STUDENT' AS account_type, 0 Private
UNION
SELECT        '999999090' AS ID_NUM, 'Rci-Test2' AS firstname, '' AS lastname, 'rci-test2@gordon.edu' AS email, 'Rci-test2' AS AD_Username, 'STUDENT' AS account_type, 0 Private
UNION
SELECT        '999999091' AS ID_NUM, 'Rci-Test3' AS firstname, '' AS lastname, 'rci-test3@gordon.edu' AS email, 'Rci-test3' AS AD_Username, 'STUDENT' AS account_type, 0 Private
UNION
SELECT        '999999092' AS ID_NUM, 'Rci-Test4' AS firstname, '' AS lastname, 'rci-test4@gordon.edu' AS email, 'Rci-test4' AS AD_Username, 'STUDENT' AS account_type, 0 Private
UNION
SELECT        '999999093' AS ID_NUM, 'Rci-Test5' AS firstname, '' AS lastname, 'rci-test5@gordon.edu' AS email, 'Rci-test5' AS AD_Username, 'STUDENT' AS account_type, 0 Private
UNION
SELECT        '999999094' AS ID_NUM, 'Rci-Test6' AS firstname, '' AS lastname, 'rci-test6@gordon.edu' AS email, 'Rci-test6' AS AD_Username, 'STUDENT' AS account_type, 0 Private
UNION
SELECT        '999999095' AS ID_NUM, 'Rci-Test7' AS firstname, '' AS lastname, 'rci-test7@gordon.edu' AS email, 'Rci-test7' AS AD_Username, 'STAFF' AS account_type, 0 Private





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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Account'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Account'
GO
