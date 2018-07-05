USE [RCITrain]

/****** Object:  Table [dbo].[Account]    Script Date: 6/26/2018 11:13:01 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[Account](
	[ID_NUM] [varchar](10) NOT NULL,
	[firstname] [varchar](50) NULL,
	[lastname] [varchar](50) NULL,
	[email] [varchar](50) NOT NULL,
	[AD_Username] [varchar](50) NULL,
	[account_type] [varchar](20) NULL,
	[Private] [int] NOT NULL
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[Admin]    Script Date: 6/26/2018 11:13:01 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Admin](
	[GordonID] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[GordonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[BuildingAssign]    Script Date: 6/26/2018 11:13:01 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[BuildingAssign](
	[JobTitleHall] [nvarchar](50) NOT NULL,
	[BuildingCode] [nvarchar](50) NOT NULL
) ON [PRIMARY]


/****** Object:  Table [dbo].[CommonAreaRciSignature]    Script Date: 6/26/2018 11:13:01 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[CommonAreaRciSignature](
	[GordonID] [nvarchar](50) NOT NULL,
	[RciID] [int] NOT NULL,
	[Signature] [date] NULL,
	[SignatureType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CommonAreaRciSignature] PRIMARY KEY CLUSTERED 
(
	[GordonID] ASC,
	[RciID] ASC,
	[SignatureType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[CurrentRA]    Script Date: 6/26/2018 11:13:02 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[CurrentRA](
	[Dorm] [varchar](5) NOT NULL,
	[AD_Username] [varchar](255) NOT NULL,
	[ID_NUM] [int] NOT NULL
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[CurrentRD]    Script Date: 6/26/2018 11:13:02 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[CurrentRD](
	[ID_NUM] [varchar](10) NOT NULL,
	[firstname] [varchar](50) NULL,
	[lastname] [varchar](50) NULL,
	[email] [varchar](50) NULL,
	[Job_Title] [varchar](512) NULL,
	[Job_Title_Hall] [varchar](512) NULL
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[Damage]    Script Date: 6/26/2018 11:13:02 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Damage](
	[DamageID] [int] IDENTITY(1,1) NOT NULL,
	[DamageDescription] [nvarchar](max) NULL,
	[DamageImagePath] [nvarchar](max) NULL,
	[DamageType] [nvarchar](50) NOT NULL,
	[RciComponentID] [int] NOT NULL,
	[FineAssessed] [decimal](10, 4) NULL,
 CONSTRAINT [PK_Damage] PRIMARY KEY CLUSTERED 
(
	[DamageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


/****** Object:  Table [dbo].[Fine]    Script Date: 6/26/2018 11:13:02 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Fine](
	[FineID] [int] IDENTITY(1,1) NOT NULL,
	[FineAmount] [decimal](13, 2) NOT NULL,
	[GordonID] [nvarchar](50) NULL,
	[Reason] [nvarchar](max) NOT NULL,
	[RciComponentID] [int] NULL,
 CONSTRAINT [PK_Fines] PRIMARY KEY CLUSTERED 
(
	[FineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


/****** Object:  Table [dbo].[Rci]    Script Date: 6/26/2018 11:13:02 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Rci](
	[RciID] [int] IDENTITY(1,1) NOT NULL,
	[IsCurrent] [bit] NULL,
	[CreationDate] [datetime] NULL,
	[BuildingCode] [nvarchar](50) NOT NULL,
	[RoomNumber] [nvarchar](50) NOT NULL,
	[GordonID] [nvarchar](50) NULL,
	[SessionCode] [nvarchar](50) NULL,
	[CheckinSigRes] [date] NULL,
	[CheckinSigRA] [date] NULL,
	[CheckinSigRD] [date] NULL,
	[LifeAndConductSigRes] [date] NULL,
	[CheckoutSigRes] [date] NULL,
	[CheckoutSigRA] [date] NULL,
	[CheckoutSigRD] [date] NULL,
	[CheckoutSigRAGordonID] [nvarchar](50) NULL,
	[CheckoutSigRDGordonID] [nvarchar](50) NULL,
	[CheckinSigRAGordonID] [nvarchar](50) NULL,
	[CheckinSigRDGordonID] [nvarchar](50) NULL,
 CONSTRAINT [PK_RCI] PRIMARY KEY CLUSTERED 
(
	[RciID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Table [dbo].[RciComponent]    Script Date: 6/26/2018 11:13:03 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[RciComponent](
	[RciComponentID] [int] IDENTITY(1,1) NOT NULL,
	[RciComponentName] [nvarchar](50) NOT NULL,
	[RciID] [int] NOT NULL,
	[RciComponentDescription] [nvarchar](100) NULL,
	[SuggestedCosts] [nvarchar](max) NULL,
 CONSTRAINT [PK_RCIComponent] PRIMARY KEY CLUSTERED 
(
	[RciComponentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


/****** Object:  Table [dbo].[RoomAssign]    Script Date: 6/26/2018 11:13:03 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[RoomAssign](
	[APPID] [int] NOT NULL,
	[SESS_CDE] [varchar](8) NOT NULL,
	[BLDG_LOC_CDE] [varchar](5) NOT NULL,
	[BLDG_CDE] [varchar](5) NOT NULL,
	[ROOM_CDE] [varchar](5) NOT NULL,
	[ROOM_SLOT_NUM] [int] NOT NULL,
	[ID_NUM] [int] NULL,
	[ROOM_TYPE] [varchar](2) NULL,
	[ROOM_ASSIGN_STS] [varchar](1) NOT NULL,
	[ASSIGN_DTE] [datetime] NULL,
	[APPROWVERSION] [timestamp] NOT NULL,
	[USER_NAME] [varchar](513) NULL,
	[JOB_NAME] [varchar](30) NULL,
	[JOB_TIME] [datetime] NULL
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[RoomChangeHistory]    Script Date: 6/26/2018 11:13:03 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING OFF

CREATE TABLE [dbo].[RoomChangeHistory](
	[SESS_CDE] [char](8) NOT NULL,
	[ID_NUM] [int] NOT NULL,
	[ROOM_CHANGE_DTE] [datetime] NOT NULL,
	[OLD_BLDG_LOC_CDE] [char](5) NULL,
	[OLD_BLDG_CDE] [char](5) NULL,
	[OLD_ROOM_CDE] [char](5) NULL,
	[NEW_BLDG_LOC_CDE] [char](5) NULL,
	[NEW_BLDG_CDE] [char](5) NULL,
	[NEW_ROOM_CDE] [char](5) NULL,
	[ROOM_CHANGE_REASON] [char](3) NULL,
	[ROOM_CHANGE_REASON_DESC] [char](60) NULL,
	[ROOM_CHANGE_COMMENT] [varchar](255) NULL
) ON [PRIMARY]


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[Room]    Script Date: 6/26/2018 11:13:03 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[Room](
	[LOC_CDE] [varchar](5) NOT NULL,
	[BLDG_CDE] [varchar](5) NOT NULL,
	[BUILDING_DESC] [varchar](45) NULL,
	[ROOM_CDE] [varchar](5) NOT NULL,
	[ROOM_DESC] [varchar](45) NULL,
	[ROOM_TYPE] [varchar](2) NULL,
	[ROOM_TYPE_DESC] [varchar](60) NULL,
	[MAX_CAPACITY] [int] NOT NULL
) ON [PRIMARY]
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[Room] ADD [ROOM_GENDER] [char](1) NULL
ALTER TABLE [dbo].[Room] ADD [RM_WHICH_FLOOR] [int] NULL


SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[Session]    Script Date: 6/26/2018 11:13:04 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING OFF

CREATE TABLE [dbo].[Session](
	[SESS_CDE] [char](8) NOT NULL,
	[SESS_DESC] [varchar](1000) NULL,
	[SESS_BEGN_DTE] [datetime] NULL,
	[SESS_END_DTE] [datetime] NULL
) ON [PRIMARY]


SET ANSI_PADDING OFF

SET ANSI_PADDING ON


/****** Object:  Index [IX_Rci_BuildingCode]    Script Date: 6/26/2018 11:13:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rci_BuildingCode] ON [dbo].[Rci]
(
	[BuildingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON


/****** Object:  Index [IX_Rci_GordonID]    Script Date: 6/26/2018 11:13:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rci_GordonID] ON [dbo].[Rci]
(
	[GordonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON


/****** Object:  Index [IX_Rci_RoomNumber]    Script Date: 6/26/2018 11:13:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rci_RoomNumber] ON [dbo].[Rci]
(
	[RoomNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

SET ANSI_PADDING ON


/****** Object:  Index [IX_Rci_SessionCode]    Script Date: 6/26/2018 11:13:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rci_SessionCode] ON [dbo].[Rci]
(
	[SessionCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE [dbo].[CommonAreaRciSignature]  WITH CHECK ADD  CONSTRAINT [FK_CommonAreaRciSignature_Rci] FOREIGN KEY([RciID])
REFERENCES [dbo].[Rci] ([RciID])
ON DELETE CASCADE

ALTER TABLE [dbo].[CommonAreaRciSignature] CHECK CONSTRAINT [FK_CommonAreaRciSignature_Rci]

ALTER TABLE [dbo].[Damage]  WITH CHECK ADD  CONSTRAINT [FK_Damage_RCIComponent] FOREIGN KEY([RciComponentID])
REFERENCES [dbo].[RciComponent] ([RciComponentID])
ON DELETE CASCADE

ALTER TABLE [dbo].[Damage] CHECK CONSTRAINT [FK_Damage_RCIComponent]

ALTER TABLE [dbo].[Fine]  WITH CHECK ADD  CONSTRAINT [FK_Fines_RCIComponent] FOREIGN KEY([RciComponentID])
REFERENCES [dbo].[RciComponent] ([RciComponentID])
ON DELETE CASCADE

ALTER TABLE [dbo].[Fine] CHECK CONSTRAINT [FK_Fines_RCIComponent]

ALTER TABLE [dbo].[RciComponent]  WITH CHECK ADD  CONSTRAINT [FK_RCIComponent_RCI] FOREIGN KEY([RciID])
REFERENCES [dbo].[Rci] ([RciID])
ON DELETE CASCADE

ALTER TABLE [dbo].[RciComponent] CHECK CONSTRAINT [FK_RCIComponent_RCI]


