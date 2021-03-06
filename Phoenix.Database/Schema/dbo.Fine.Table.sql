USE [RCITrain]
GO
/****** Object:  Table [dbo].[Fine]    Script Date: 6/26/2018 9:34:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
ALTER TABLE [dbo].[Fine]  WITH CHECK ADD  CONSTRAINT [FK_Fines_RCIComponent] FOREIGN KEY([RciComponentID])
REFERENCES [dbo].[RciComponent] ([RciComponentID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Fine] CHECK CONSTRAINT [FK_Fines_RCIComponent]
GO
