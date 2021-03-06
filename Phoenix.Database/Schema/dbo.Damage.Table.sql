USE [RCITrain]
GO
/****** Object:  Table [dbo].[Damage]    Script Date: 6/26/2018 9:34:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
ALTER TABLE [dbo].[Damage]  WITH CHECK ADD  CONSTRAINT [FK_Damage_RCIComponent] FOREIGN KEY([RciComponentID])
REFERENCES [dbo].[RciComponent] ([RciComponentID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Damage] CHECK CONSTRAINT [FK_Damage_RCIComponent]
GO
