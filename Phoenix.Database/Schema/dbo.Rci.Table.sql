USE [RCITrain]
GO
/****** Object:  Table [dbo].[Rci]    Script Date: 6/26/2018 9:34:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Rci_BuildingCode]    Script Date: 6/26/2018 9:34:03 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rci_BuildingCode] ON [dbo].[Rci]
(
	[BuildingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Rci_GordonID]    Script Date: 6/26/2018 9:34:03 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rci_GordonID] ON [dbo].[Rci]
(
	[GordonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Rci_RoomNumber]    Script Date: 6/26/2018 9:34:03 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rci_RoomNumber] ON [dbo].[Rci]
(
	[RoomNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Rci_SessionCode]    Script Date: 6/26/2018 9:34:03 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rci_SessionCode] ON [dbo].[Rci]
(
	[SessionCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
