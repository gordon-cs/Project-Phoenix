USE [RCITrain]
GO
/****** Object:  Table [dbo].[BuildingAssign]    Script Date: 6/26/2018 9:49:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BuildingAssign](
	[JobTitleHall] [nvarchar](50) NOT NULL,
	[BuildingCode] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Wilson and Lewis', N'WIL')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Wilson and Lewis', N'LEW')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Fulton', N'FUL')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Ferrin and Drew', N'FER')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Ferrin and Drew', N'DRW')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Chase', N'CHA')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Bromley', N'BRO')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Road Halls', N'GED')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Road Halls', N'CON')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Road Halls', N'GRA')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Road Halls', N'HIL')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Road Halls', N'MCI')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Road Halls', N'RID')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Nyland', N'NYL')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Tavilla', N'TAV')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Evans', N'EVN')
INSERT [dbo].[BuildingAssign] ([JobTitleHall], [BuildingCode]) VALUES (N'Road Halls', N'DEX')
