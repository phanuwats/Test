﻿/****** Object:  StoredProcedure [dbo].[SP_TRN_TRANSACTION_SELECT_TRNNO_COUNT]    Script Date: 5/16/2021 12:35:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_TRN_TRANSACTION_SELECT_TRNNO_COUNT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_TRN_TRANSACTION_SELECT_TRNNO_COUNT]
GO
/****** Object:  StoredProcedure [dbo].[SP_TRN_TRANSACTION_SELECT]    Script Date: 5/16/2021 12:35:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_TRN_TRANSACTION_SELECT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_TRN_TRANSACTION_SELECT]
GO
/****** Object:  StoredProcedure [dbo].[SP_TRN_TRANSACTION_INSERT]    Script Date: 5/16/2021 12:35:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_TRN_TRANSACTION_INSERT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_TRN_TRANSACTION_INSERT]
GO
/****** Object:  StoredProcedure [dbo].[SP_MAS_LOOKUP_SELECT]    Script Date: 5/16/2021 12:35:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_MAS_LOOKUP_SELECT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_MAS_LOOKUP_SELECT]
GO
/****** Object:  StoredProcedure [dbo].[SP_LOG_IMPORT_INSERT]    Script Date: 5/16/2021 12:35:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_LOG_IMPORT_INSERT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_LOG_IMPORT_INSERT]
GO
/****** Object:  Table [dbo].[TB_TRN_TRANSACTION]    Script Date: 5/16/2021 12:35:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TB_TRN_TRANSACTION]') AND type in (N'U'))
DROP TABLE [dbo].[TB_TRN_TRANSACTION]
GO
/****** Object:  Table [dbo].[TB_MAS_LOOKUP]    Script Date: 5/16/2021 12:35:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TB_MAS_LOOKUP]') AND type in (N'U'))
DROP TABLE [dbo].[TB_MAS_LOOKUP]
GO
/****** Object:  Table [dbo].[TB_LOG_IMPORT]    Script Date: 5/16/2021 12:35:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TB_LOG_IMPORT]') AND type in (N'U'))
DROP TABLE [dbo].[TB_LOG_IMPORT]
GO
/****** Object:  Table [dbo].[TB_LOG_IMPORT]    Script Date: 5/16/2021 12:35:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_LOG_IMPORT](
	[IMP_LOG_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FILE_NAME] [varchar](200) NULL,
	[ERROR_MESSAGE] [text] NULL,
	[STATUS] [bit] NULL,
	[CREATE_BY] [varchar](50) NULL,
	[CREATE_DATE] [datetime] NULL,
	[UPDATE_BY] [varchar](50) NULL,
	[UPDATE_DATE] [datetime] NULL,
 CONSTRAINT [PK_TB_LOG_IMPORT] PRIMARY KEY CLUSTERED 
(
	[IMP_LOG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_MAS_LOOKUP]    Script Date: 5/16/2021 12:35:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_MAS_LOOKUP](
	[LOOKUP_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LOOKUP_GROUP] [varchar](50) NULL,
	[LOOKUP_NAME] [varchar](50) NULL,
	[LOOKUP_CODE] [varchar](50) NULL,
 CONSTRAINT [PK_TB_MAS_LOOKUP] PRIMARY KEY CLUSTERED 
(
	[LOOKUP_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TB_TRN_TRANSACTION]    Script Date: 5/16/2021 12:35:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TB_TRN_TRANSACTION](
	[TRN_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[IMP_LOG_ID] [bigint] NULL,
	[TRN_NO] [varchar](50) NULL,
	[AMOUNT] [decimal](18, 2) NULL,
	[CUR_CODE] [varchar](3) NULL,
	[TRN_DATE] [datetime] NULL,
	[STATUS] [char](1) NULL,
	[IS_ACTIVE] [bit] NULL,
	[CREATE_BY] [varchar](50) NULL,
	[CREATE_DATE] [datetime] NULL,
	[UPDATE_BY] [varchar](50) NULL,
	[UPDATE_DATE] [datetime] NULL,
 CONSTRAINT [PK_TB_TRN_TRANSACTION] PRIMARY KEY CLUSTERED 
(
	[TRN_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[SP_LOG_IMPORT_INSERT]    Script Date: 5/16/2021 12:35:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_LOG_IMPORT_INSERT]
	@FILE_NAME varchar(200),
    @ERROR_MESSAGE text,
    @STATUS bit,
    @CREATE_BY varchar(50)
AS
BEGIN
	

	SET NOCOUNT ON;

    INSERT INTO [dbo].[TB_LOG_IMPORT]
           ([FILE_NAME]
           ,[ERROR_MESSAGE]
           ,[STATUS]
           ,[CREATE_BY]
           ,[CREATE_DATE]
           ,[UPDATE_BY]
           ,[UPDATE_DATE])
     VALUES
           (@FILE_NAME
           ,@ERROR_MESSAGE
           ,@STATUS
           ,@CREATE_BY
           ,GETDATE()
           ,@CREATE_BY
           ,GETDATE())

	SELECT @@IDENTITY

END
GO
/****** Object:  StoredProcedure [dbo].[SP_MAS_LOOKUP_SELECT]    Script Date: 5/16/2021 12:35:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_MAS_LOOKUP_SELECT] 
	@LOOKUP_GROUP varchar(50)
AS
BEGIN
	
	SET NOCOUNT ON;

    SELECT [LOOKUP_GROUP]
      ,[LOOKUP_NAME]
      ,[LOOKUP_CODE]
	FROM [dbo].[TB_MAS_LOOKUP]
	WHERE [LOOKUP_GROUP] = @LOOKUP_GROUP 


END
GO
/****** Object:  StoredProcedure [dbo].[SP_TRN_TRANSACTION_INSERT]    Script Date: 5/16/2021 12:35:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TRN_TRANSACTION_INSERT]
	@IMP_LOG_ID bigint,
	@TRN_NO varchar(50),
    @AMOUNT decimal(18,2),
	@CUR_CODE varchar(30),
    @TRN_DATE varchar(30),
    @STATUS char(1),
    @CREATE_BY varchar(50)
AS
BEGIN
	
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TB_TRN_TRANSACTION]
           ([IMP_LOG_ID]
		   ,[TRN_NO]
           ,[AMOUNT]
		   ,[CUR_CODE]
           ,[TRN_DATE]
           ,[STATUS]
           ,[IS_ACTIVE]
           ,[CREATE_BY]
           ,[CREATE_DATE]
           ,[UPDATE_BY]
           ,[UPDATE_DATE])
     VALUES
           (@IMP_LOG_ID
		   ,@TRN_NO
           ,@AMOUNT
		   ,@CUR_CODE
           ,CONVERT(DATETIME, @TRN_DATE, 103)
           ,@STATUS
           ,1
           ,@CREATE_BY
           ,GETDATE()
           ,@CREATE_BY
           ,GETDATE())

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TRN_TRANSACTION_SELECT]    Script Date: 5/16/2021 12:35:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--EXEC SP_TRN_TRANSACTION_SELECT @S_TRN_DATE = '01/10/2000'
CREATE PROCEDURE [dbo].[SP_TRN_TRANSACTION_SELECT]
	@CUR_CODE varchar(3) = NULL,
	@S_TRN_DATE varchar(20) = NULL,
	@E_TRN_DATE varchar(20) = NULL,
	@STATUS CHAR(1) = NULL
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT [TRN_NO] [id]
      ,CONVERT(VARCHAR, [AMOUNT])+' '+[CUR_CODE]  [payment]
      ,[STATUS] [Status]
	FROM [dbo].[TB_TRN_TRANSACTION]
	WHERE (@CUR_CODE IS NULL OR [CUR_CODE] = @CUR_CODE) AND
		(@S_TRN_DATE IS NULL OR [TRN_DATE] >= CONVERT(DATETIME, @S_TRN_DATE +' 00:00:00', 103)) AND 
		(@E_TRN_DATE IS NULL OR [TRN_DATE] <= CONVERT(DATETIME, @E_TRN_DATE +' 23:59:59', 103)) AND 
		(@STATUS IS NULL OR [STATUS] = @STATUS)
	ORDER BY [TRN_NO]


END
GO
/****** Object:  StoredProcedure [dbo].[SP_TRN_TRANSACTION_SELECT_TRNNO_COUNT]    Script Date: 5/16/2021 12:35:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TRN_TRANSACTION_SELECT_TRNNO_COUNT]
	@TRN_NO varchar(50)
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT COUNT([TRN_NO])
	FROM [dbo].[TB_TRN_TRANSACTION]
	WHERE [TRN_NO] = @TRN_NO

END
GO
