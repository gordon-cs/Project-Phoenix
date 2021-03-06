USE [RCITrain]
GO
/****** Object:  Table [dbo].[RciComponent]    Script Date: 6/26/2018 9:34:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
ALTER TABLE [dbo].[RciComponent]  WITH CHECK ADD  CONSTRAINT [FK_RCIComponent_RCI] FOREIGN KEY([RciID])
REFERENCES [dbo].[Rci] ([RciID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RciComponent] CHECK CONSTRAINT [FK_RCIComponent_RCI]
GO
