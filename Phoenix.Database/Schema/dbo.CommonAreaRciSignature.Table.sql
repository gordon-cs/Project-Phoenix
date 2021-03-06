USE [RCITrain]
GO
/****** Object:  Table [dbo].[CommonAreaRciSignature]    Script Date: 6/26/2018 9:34:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
ALTER TABLE [dbo].[CommonAreaRciSignature]  WITH CHECK ADD  CONSTRAINT [FK_CommonAreaRciSignature_Rci] FOREIGN KEY([RciID])
REFERENCES [dbo].[Rci] ([RciID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CommonAreaRciSignature] CHECK CONSTRAINT [FK_CommonAreaRciSignature_Rci]
GO
