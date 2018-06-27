USE [RCITrain]
GO
/****** Object:  Table [dbo].[CurrentRD]    Script Date: 6/26/2018 9:49:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CurrentRD](
	[ID_NUM] [varchar](10) NOT NULL,
	[firstname] [varchar](50) NULL,
	[lastname] [varchar](50) NULL,
	[email] [varchar](50) NULL,
	[Job_Title] [varchar](512) NULL,
	[Job_Title_Hall] [varchar](512) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'40000638', N'Klamesha', N'Richards', N'Klamesha.Richards@gordon.edu', N'Resident Director - Fulton', N'Fulton')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'40000911', N'Ethan', N'Mignard', N'Ethan.Mignard@gordon.edu', N'Resident Director - Tavilla', N'Tavilla')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'40000912', N'Jeff', N'Carpenter', N'Jeff.Carpenter@gordon.edu', N'Resident Director - Nyland', N'Nyland')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'40000930', N'Sok', N'Son', N'Tim.Son@gordon.edu', N'Resident Director - Evans', N'Evans')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'9466471', N'Pollyanna', N'Woods', N'Pollyanna.Woods@gordon.edu', N'Resident Director - Chase', N'Chase')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'9484562', N'Yicaury', N'Melo Jimenez', N'Yicaury.Melo@gordon.edu', N'Resident Director - Bromley', N'Bromley')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'9529423', N'Sarah', N'Welch', N'Sarah.J.Welch@gordon.edu', N'Resident Director - Road Halls', N'Road Halls')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'9582572', N'Grace', N'Gossett', N'Grace.Gossett@gordon.edu', N'Resident Director - Ferrin and Drew', N'Ferrin and Drew')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'999999095', N'Rci-Test7', N'', N'rci-test7@gordon.edu', N'Resident Director - Tavilla', N'Tavilla')
INSERT [dbo].[CurrentRD] ([ID_NUM], [firstname], [lastname], [email], [Job_Title], [Job_Title_Hall]) VALUES (N'999999098', N'360', N'StaffTest', N'360.StaffTest@gordon.edu', N'Resident Director - Wilson and Lewis', N'Wilson and Lewis')
