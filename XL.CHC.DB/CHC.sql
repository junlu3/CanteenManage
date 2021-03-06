USE [master]
GO
/****** Object:  Database [Husky]    Script Date: 1/15/2017 12:42:55 PM ******/
CREATE DATABASE [Husky]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Husky_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Husky_Data.mdf' , SIZE = 12288KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)
 LOG ON 
( NAME = N'Husky_Log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Husky_Log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 1024KB )
GO
ALTER DATABASE [Husky] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Husky].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Husky] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Husky] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Husky] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Husky] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Husky] SET ARITHABORT OFF 
GO
ALTER DATABASE [Husky] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Husky] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Husky] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Husky] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Husky] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Husky] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Husky] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Husky] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Husky] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Husky] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Husky] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Husky] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Husky] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Husky] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Husky] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Husky] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Husky] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Husky] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Husky] SET RECOVERY FULL 
GO
ALTER DATABASE [Husky] SET  MULTI_USER 
GO
ALTER DATABASE [Husky] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Husky] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Husky] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Husky] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Husky', N'ON'
GO
USE [Husky]
GO
/****** Object:  Table [dbo].[AutoTask]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AutoTask](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[AutoTaskStatus] [int] NOT NULL,
	[AutoTaskType] [int] NOT NULL,
	[LastRunStartTime] [smalldatetime] NULL,
	[Comment] [nvarchar](200) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Category]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[CategoryType_Id] [int] NULL,
	[DisplayOrder] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CategoryType]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategoryType](
	[Id] [int] NOT NULL,
	[TypeName] [nvarchar](50) NULL,
	[Description] [nvarchar](200) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Company]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[Id] [uniqueidentifier] NOT NULL,
	[MembershipUser_Id] [uniqueidentifier] NULL,
	[CompanyName] [nvarchar](100) NULL,
	[CompanyAddress] [nvarchar](255) NULL,
	[Telephone] [nvarchar](50) NULL,
	[ZipCode] [nvarchar](50) NULL,
	[Fax] [nvarchar](50) NULL,
	[CompanyType] [int] NULL,
	[LegalPerson] [nvarchar](50) NULL,
	[ContactPerson] [nvarchar](50) NULL,
	[ContactPhone] [nvarchar](50) NULL,
	[RestDay] [nvarchar](50) NULL,
	[RegisterDate] [smalldatetime] NULL,
	[CompanyRegisterType] [int] NULL,
	[Deleted] [bit] NULL,
	[CreatedDate] [smalldatetime] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[IsDefault] [bit] NULL,
	[CompanyStamp] [nvarchar](2000) NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Email]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Email](
	[Id] [uniqueidentifier] NOT NULL,
	[To] [nvarchar](500) NOT NULL,
	[ToName] [nvarchar](500) NULL,
	[ReplyTo] [nvarchar](500) NULL,
	[ReplyToName] [nvarchar](500) NULL,
	[CC] [nvarchar](500) NULL,
	[Bcc] [nvarchar](500) NULL,
	[Subject] [nvarchar](500) NULL,
	[Body] [nvarchar](max) NULL,
	[AttachmentFilePath] [nvarchar](max) NULL,
	[AttachmentFileName] [nvarchar](max) NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[CreatedBy] [nvarchar](1000) NOT NULL,
	[SentTries] [int] NOT NULL,
	[SentDate] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HR_EMPLOYEE_OLD]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HR_EMPLOYEE_OLD](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EMPLOYEE_CARD] [nvarchar](50) NULL,
	[ROW_ID] [nvarchar](50) NULL,
	[EMPLOYEE_NAME] [nvarchar](50) NULL,
	[EMPLOYEE_PY] [nvarchar](50) NULL,
	[IS_CHINESE_FOOD] [bit] NULL,
	[IS_WEST_FOOD] [bit] NULL,
	[IS_SPECIAL_FOOD] [bit] NULL,
	[IS_COFFEE] [bit] NULL,
	[CARD_STATUS] [int] NULL,
 CONSTRAINT [PK_HR_EMPLOYEE] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HU_MEAL_RECORD]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HU_MEAL_RECORD](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CREATED_PROGRAM] [nvarchar](100) NULL,
	[GROUP_ID] [nvarchar](50) NULL,
	[COMPANY_ID] [nvarchar](100) NULL,
	[EMPLOYEE_ID] [uniqueidentifier] NOT NULL,
	[MEAL_DATETIME] [datetime] NULL,
	[MEAL_TYPE_CODE] [nvarchar](100) NULL,
	[MEAL_PRICE] [decimal](18, 1) NULL,
	[QUANTITY] [int] NULL,
	[MEAL_CLASS] [nvarchar](100) NULL,
	[MEAL_AMOUNT] [decimal](18, 1) NULL,
	[IMPORT_DATA_ID] [nvarchar](100) NULL,
	[CREATED_DATE] [datetime] NULL,
	[TAG] [int] NULL,
	[DELETE_FLAG] [bit] NULL,
	[CREATED_BY] [nvarchar](50) NULL,
	[ROW_ID] [nvarchar](100) NULL,
	[MEAL_TYPE_NAME] [nvarchar](100) NULL,
	[MEAL_TYPE_ID] [nvarchar](100) NULL,
	[EXTRA_MEAL] [bit] NULL,
 CONSTRAINT [PK_HU_MEAL_RECORD] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HU_MEAL_TYPE]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HU_MEAL_TYPE](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MEAL_TYPE_CODE] [nvarchar](50) NULL,
	[MEAL_PRICE] [decimal](18, 1) NULL,
	[DELETE_FLAG] [bit] NULL,
	[ROW_ID] [nvarchar](50) NULL,
	[MEAL_TYPE_NAME] [nvarchar](50) NULL,
 CONSTRAINT [PK_HU_MEAL_TYPE] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MembershipRole]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MembershipRole](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[Company_Id] [uniqueidentifier] NULL,
	[Create_Date] [datetime] NULL,
	[Create_By] [nvarchar](100) NULL,
	[Update_Date] [datetime] NULL,
	[Update_By] [nvarchar](100) NULL,
 CONSTRAINT [PK_MembershipRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MembershipUser]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MembershipUser](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](1000) NOT NULL,
	[Email] [nvarchar](1000) NULL,
	[Password] [nvarchar](max) NOT NULL,
	[AdminComment] [nvarchar](max) NULL,
	[Deleted] [bit] NOT NULL,
	[LastIpAddress] [nvarchar](max) NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
	[LastLoginDate] [smalldatetime] NULL,
	[BirthDate] [smalldatetime] NULL,
	[Phone] [nvarchar](20) NULL,
	[IsActived] [bit] NOT NULL,
	[QQ] [nvarchar](20) NULL,
	[WeiXin] [nvarchar](20) NULL,
	[Company_Id] [uniqueidentifier] NULL,
 CONSTRAINT [PK_MembershipUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MembershipUser_EmailTaskType_Mapping]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MembershipUser_EmailTaskType_Mapping](
	[Id] [uniqueidentifier] NOT NULL,
	[MembershipUser_Id] [uniqueidentifier] NULL,
	[EmailTaskType_Id] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MembershipUser_MembershipRole_Mapping]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MembershipUser_MembershipRole_Mapping](
	[MembershipUser_Id] [uniqueidentifier] NOT NULL,
	[MembershipRole_Id] [uniqueidentifier] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MenuItem]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MenuItem](
	[Id] [int] NOT NULL,
	[MenuName] [nvarchar](50) NULL,
	[MenuText] [nvarchar](100) NULL,
	[ParentId] [int] NOT NULL,
	[Url] [nvarchar](100) NULL,
	[MenuOrder] [int] NULL,
	[Controllor] [nvarchar](50) NULL,
	[IconClass] [nvarchar](50) NULL,
 CONSTRAINT [PK_MenuItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MenuItem_MembershipRole_Mapping]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MenuItem_MembershipRole_Mapping](
	[MenuItem_Id] [int] NOT NULL,
	[MembershipRole_Id] [uniqueidentifier] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MSDS_Customer]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MSDS_Customer](
	[ROW_ID] [uniqueidentifier] NOT NULL,
	[EMPLOYEE_CARD] [nvarchar](50) NULL,
	[COMPANY_CODE] [nvarchar](100) NULL,
	[EMPLOYEE_ID] [nvarchar](100) NULL,
	[EMPLOYEE_NAME] [nvarchar](100) NULL,
	[EMPLOYEE_NAME_CN] [nvarchar](100) NULL,
	[EMPLOYEE_NAME_EN] [nvarchar](100) NULL,
	[DEPARTMENT_NAME] [nvarchar](100) NULL,
	[MGR_NAME] [nvarchar](100) NULL,
	[LOCATION] [nvarchar](100) NULL,
	[COMBO_CODE] [nvarchar](100) NULL,
	[IS_CHINESE_FOOD] [bit] NOT NULL,
	[IS_WEST_FOOD] [bit] NOT NULL,
	[IS_SPECIAL_FOOD] [bit] NOT NULL,
	[IS_COFFEE] [bit] NOT NULL,
	[EMPLOYEE_PY] [nvarchar](50) NULL,
	[CARD_STATUS] [bit] NULL,
	[LastMealTime] [datetime] NULL,
	[IS_BREAKFAST] [bit] NULL,
 CONSTRAINT [PK_MSDS_Customer] PRIMARY KEY CLUSTERED 
(
	[ROW_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MSDS_Customer_old]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MSDS_Customer_old](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](50) NULL,
	[Address] [nvarchar](200) NULL,
	[HeadPic] [nvarchar](200) NULL,
	[Enabled] [bit] NOT NULL,
	[Type] [int] NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[CreateTime] [datetime] NULL,
	[CreateBy] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PermissionRecord]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermissionRecord](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[Alias] [nvarchar](50) NULL,
	[PageUrl] [nvarchar](200) NULL,
	[CreatedDate] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PermissionRecord_MembershipRole_Mapping]    Script Date: 1/15/2017 12:42:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermissionRecord_MembershipRole_Mapping](
	[PermissionRecord_Id] [uniqueidentifier] NOT NULL,
	[MembershipRole_Id] [uniqueidentifier] NOT NULL
) ON [PRIMARY]

GO
INSERT [dbo].[Company] ([Id], [MembershipUser_Id], [CompanyName], [CompanyAddress], [Telephone], [ZipCode], [Fax], [CompanyType], [LegalPerson], [ContactPerson], [ContactPhone], [RestDay], [RegisterDate], [CompanyRegisterType], [Deleted], [CreatedDate], [CreatedBy], [IsDefault], [CompanyStamp]) VALUES (N'39095392-50bb-4227-b426-26b310af05f5', N'aff25fd3-c567-4a6a-8cfb-90561c5fdddd', N'Husky', NULL, NULL, NULL, NULL, 36, NULL, NULL, NULL, N'双休日', NULL, 35, 1, CAST(0xA58D0547 AS SmallDateTime), NULL, 0, N'/Content/Images/CompanyStamps/635884074725366808.png')
INSERT [dbo].[Company] ([Id], [MembershipUser_Id], [CompanyName], [CompanyAddress], [Telephone], [ZipCode], [Fax], [CompanyType], [LegalPerson], [ContactPerson], [ContactPhone], [RestDay], [RegisterDate], [CompanyRegisterType], [Deleted], [CreatedDate], [CreatedBy], [IsDefault], [CompanyStamp]) VALUES (N'42d1490c-99fd-4dc8-9de9-63702806bf47', N'ba701e61-61dd-4111-bfcc-d0b28c62c53f', N'系统', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, CAST(0xA56F03AC AS SmallDateTime), NULL, 1, NULL)
SET IDENTITY_INSERT [dbo].[HR_EMPLOYEE_OLD] ON 


SET IDENTITY_INSERT [dbo].[HR_EMPLOYEE_OLD] OFF
SET IDENTITY_INSERT [dbo].[HU_MEAL_TYPE] ON 

INSERT [dbo].[HU_MEAL_TYPE] ([ID], [MEAL_TYPE_CODE], [MEAL_PRICE], [DELETE_FLAG], [ROW_ID], [MEAL_TYPE_NAME]) VALUES (1, N'chinese food', CAST(10.5 AS Decimal(18, 1)), 0, NULL, NULL)
INSERT [dbo].[HU_MEAL_TYPE] ([ID], [MEAL_TYPE_CODE], [MEAL_PRICE], [DELETE_FLAG], [ROW_ID], [MEAL_TYPE_NAME]) VALUES (2, N'western food', CAST(15.0 AS Decimal(18, 1)), 0, NULL, NULL)
INSERT [dbo].[HU_MEAL_TYPE] ([ID], [MEAL_TYPE_CODE], [MEAL_PRICE], [DELETE_FLAG], [ROW_ID], [MEAL_TYPE_NAME]) VALUES (3, N'special food', CAST(10.5 AS Decimal(18, 1)), 0, NULL, NULL)
INSERT [dbo].[HU_MEAL_TYPE] ([ID], [MEAL_TYPE_CODE], [MEAL_PRICE], [DELETE_FLAG], [ROW_ID], [MEAL_TYPE_NAME]) VALUES (4, N'supper', CAST(10.5 AS Decimal(18, 1)), 0, NULL, NULL)
INSERT [dbo].[HU_MEAL_TYPE] ([ID], [MEAL_TYPE_CODE], [MEAL_PRICE], [DELETE_FLAG], [ROW_ID], [MEAL_TYPE_NAME]) VALUES (5, N'coffee', CAST(5.0 AS Decimal(18, 1)), 0, NULL, NULL)
INSERT [dbo].[HU_MEAL_TYPE] ([ID], [MEAL_TYPE_CODE], [MEAL_PRICE], [DELETE_FLAG], [ROW_ID], [MEAL_TYPE_NAME]) VALUES (6, N'rice soup', CAST(0.0 AS Decimal(18, 1)), 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[HU_MEAL_TYPE] OFF
INSERT [dbo].[MembershipRole] ([Id], [Name], [Deleted], [Company_Id], [Create_Date], [Create_By], [Update_Date], [Update_By]) VALUES (N'04544256-4a44-4876-9c30-3186a71cc862', N'员工', 1, N'42d1490c-99fd-4dc8-9de9-63702806bf47', CAST(0x0000A56B00000000 AS DateTime), NULL, CAST(0x0000A5FC00D7B116 AS DateTime), N'admin')
INSERT [dbo].[MembershipRole] ([Id], [Name], [Deleted], [Company_Id], [Create_Date], [Create_By], [Update_Date], [Update_By]) VALUES (N'9279eab7-f8de-4c40-86b5-3277c94b74e3', N'审批员', 1, N'42d1490c-99fd-4dc8-9de9-63702806bf47', CAST(0x0000A5FC00D7D679 AS DateTime), N'admin', NULL, NULL)
INSERT [dbo].[MembershipRole] ([Id], [Name], [Deleted], [Company_Id], [Create_Date], [Create_By], [Update_Date], [Update_By]) VALUES (N'5f999982-937d-49ad-bc08-8e7a7fff4eb5', N'部门', 0, N'39095392-50bb-4227-b426-26b310af05f5', CAST(0x0000A6000155662D AS DateTime), N'admin', CAST(0x0000A69E00BE05AF AS DateTime), N'Husky')
INSERT [dbo].[MembershipRole] ([Id], [Name], [Deleted], [Company_Id], [Create_Date], [Create_By], [Update_Date], [Update_By]) VALUES (N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f', N'系统管理员', 0, N'42d1490c-99fd-4dc8-9de9-63702806bf47', CAST(0x0000A56A00A85D90 AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[MembershipRole] ([Id], [Name], [Deleted], [Company_Id], [Create_Date], [Create_By], [Update_Date], [Update_By]) VALUES (N'258cf461-6ca1-48bb-be2e-aab86bbff98c', N'EHS', 0, N'39095392-50bb-4227-b426-26b310af05f5', CAST(0x0000A600015407A4 AS DateTime), N'Husky', NULL, NULL)
INSERT [dbo].[MembershipRole] ([Id], [Name], [Deleted], [Company_Id], [Create_Date], [Create_By], [Update_Date], [Update_By]) VALUES (N'6ec9a092-4f5c-4e06-882e-ca8bc41190e6', N'员工', 1, N'42d1490c-99fd-4dc8-9de9-63702806bf47', CAST(0x0000A6FC00B02EF9 AS DateTime), N'admin', NULL, NULL)
INSERT [dbo].[MembershipRole] ([Id], [Name], [Deleted], [Company_Id], [Create_Date], [Create_By], [Update_Date], [Update_By]) VALUES (N'aab72563-f848-409d-afad-ecfe33bae45a', N'管理员', 0, N'39095392-50bb-4227-b426-26b310af05f5', CAST(0x0000A5FC00D66789 AS DateTime), N'admin', NULL, NULL)
INSERT [dbo].[MembershipUser] ([Id], [Username], [Email], [Password], [AdminComment], [Deleted], [LastIpAddress], [CreatedOn], [LastLoginDate], [BirthDate], [Phone], [IsActived], [QQ], [WeiXin], [Company_Id]) VALUES (N'37f495f6-cf0e-4303-99f6-2f9cc35650b9', N'Husky', N'Husky@husky.com', N'92FA25096AECFF3B', NULL, 0, NULL, CAST(0xA6FB04CF AS SmallDateTime), CAST(0xA6FB04CF AS SmallDateTime), NULL, NULL, 1, NULL, NULL, N'42d1490c-99fd-4dc8-9de9-63702806bf47')
INSERT [dbo].[MembershipUser] ([Id], [Username], [Email], [Password], [AdminComment], [Deleted], [LastIpAddress], [CreatedOn], [LastLoginDate], [BirthDate], [Phone], [IsActived], [QQ], [WeiXin], [Company_Id]) VALUES (N'ba701e61-61dd-4111-bfcc-d0b28c62c53f', N'admin', N'zhan-69@163.com', N'92FA25096AECFF3B', NULL, 0, NULL, CAST(0xA56A0000 AS SmallDateTime), CAST(0xA6FC02F6 AS SmallDateTime), NULL, NULL, 1, NULL, NULL, N'42d1490c-99fd-4dc8-9de9-63702806bf47')
INSERT [dbo].[MembershipUser_MembershipRole_Mapping] ([MembershipUser_Id], [MembershipRole_Id]) VALUES (N'37f495f6-cf0e-4303-99f6-2f9cc35650b9', N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MembershipUser_MembershipRole_Mapping] ([MembershipUser_Id], [MembershipRole_Id]) VALUES (N'ba701e61-61dd-4111-bfcc-d0b28c62c53f', N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MenuItem] ([Id], [MenuName], [MenuText], [ParentId], [Url], [MenuOrder], [Controllor], [IconClass]) VALUES (11, N'SystemManage', N'系统管理', 0, NULL, 3, N'', N'fa-cogs')
INSERT [dbo].[MenuItem] ([Id], [MenuName], [MenuText], [ParentId], [Url], [MenuOrder], [Controllor], [IconClass]) VALUES (15, N'SystemManage_UserManage', N'系统用户管理', 11, N'/MembershipUser/Manage', 1, N'MembershipUser', NULL)
INSERT [dbo].[MenuItem] ([Id], [MenuName], [MenuText], [ParentId], [Url], [MenuOrder], [Controllor], [IconClass]) VALUES (32, N'SystemManage_RoleManage', N'权限管理', 11, N'/MembershipRole/RoleManage', 5, N'MembershipRole', NULL)
INSERT [dbo].[MenuItem] ([Id], [MenuName], [MenuText], [ParentId], [Url], [MenuOrder], [Controllor], [IconClass]) VALUES (39, N'CustomerManage', N'食堂管理', 0, NULL, 4, NULL, N'fa-cogs')
INSERT [dbo].[MenuItem] ([Id], [MenuName], [MenuText], [ParentId], [Url], [MenuOrder], [Controllor], [IconClass]) VALUES (40, N'CustomerManage_CustomerManage', N'饭卡管理', 39, N'/Customer/Index', 1, N'Customer', NULL)
INSERT [dbo].[MenuItem] ([Id], [MenuName], [MenuText], [ParentId], [Url], [MenuOrder], [Controllor], [IconClass]) VALUES (41, N'CustomerManage_MealRecordManage', N'刷卡记录', 39, N'/MealRecord/Index', 2, N'MealRecord', NULL)
INSERT [dbo].[MenuItem] ([Id], [MenuName], [MenuText], [ParentId], [Url], [MenuOrder], [Controllor], [IconClass]) VALUES (42, N'CustomerManage_CoffeeManage', N'咖啡清单', 39, N'/MealRecord/MealAnalysis', 3, N'MealRecord', NULL)
INSERT [dbo].[MenuItem] ([Id], [MenuName], [MenuText], [ParentId], [Url], [MenuOrder], [Controllor], [IconClass]) VALUES (43, N'CustomerManage_ComboManage', N'成本中心结算', 39, N'/MealRecord/ComboSearch', 4, N'MealRecord', NULL)
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (43, N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (32, N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (11, N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (15, N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (11, N'258cf461-6ca1-48bb-be2e-aab86bbff98c')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (32, N'aab72563-f848-409d-afad-ecfe33bae45a')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (11, N'aab72563-f848-409d-afad-ecfe33bae45a')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (39, N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (40, N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (41, N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
INSERT [dbo].[MenuItem_MembershipRole_Mapping] ([MenuItem_Id], [MembershipRole_Id]) VALUES (42, N'7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f')
ALTER TABLE [dbo].[MembershipRole] ADD  CONSTRAINT [DF_MembershipRole_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[MembershipRole] ADD  CONSTRAINT [DF_MembershipRole_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[MembershipRole] ADD  CONSTRAINT [DF_MembershipRole_CreatedDate]  DEFAULT (getdate()) FOR [Create_Date]
GO
ALTER TABLE [dbo].[MembershipUser] ADD  CONSTRAINT [DF_MembershipUser_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[MembershipUser] ADD  CONSTRAINT [DF_MembershipUser_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [dbo].[MembershipUser] ADD  CONSTRAINT [DF_MembershipUser_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[MSDS_Customer] ADD  CONSTRAINT [DF_HR_EMPLOYEE_ROW_ID]  DEFAULT (newid()) FOR [ROW_ID]
GO
ALTER TABLE [dbo].[MSDS_Customer] ADD  CONSTRAINT [DF_MSDS_Customer_IS_CHINESE_FOOD]  DEFAULT ((0)) FOR [IS_CHINESE_FOOD]
GO
ALTER TABLE [dbo].[MSDS_Customer] ADD  CONSTRAINT [DF_MSDS_Customer_IS_WEST_FOOD]  DEFAULT ((0)) FOR [IS_WEST_FOOD]
GO
ALTER TABLE [dbo].[MSDS_Customer] ADD  CONSTRAINT [DF_MSDS_Customer_IS_SPECIAL_FOOD]  DEFAULT ((0)) FOR [IS_SPECIAL_FOOD]
GO
ALTER TABLE [dbo].[MSDS_Customer] ADD  CONSTRAINT [DF_MSDS_Customer_IS_COFFEE]  DEFAULT ((0)) FOR [IS_COFFEE]
GO
ALTER TABLE [dbo].[MSDS_Customer] ADD  CONSTRAINT [DF_HR_EMPLOYEE_CARD_STATUS]  DEFAULT ((1)) FOR [CARD_STATUS]
GO
ALTER TABLE [dbo].[MSDS_Customer_old] ADD  CONSTRAINT [DF_Husky_Customer_Enabled]  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [dbo].[MSDS_Customer_old] ADD  CONSTRAINT [DF_Husky_Customer_Type]  DEFAULT ((1)) FOR [Type]
GO
ALTER TABLE [dbo].[HU_MEAL_RECORD]  WITH CHECK ADD  CONSTRAINT [FK_HU_MEAL_RECORD_MSDS_Customer] FOREIGN KEY([EMPLOYEE_ID])
REFERENCES [dbo].[MSDS_Customer] ([ROW_ID])
GO
ALTER TABLE [dbo].[HU_MEAL_RECORD] CHECK CONSTRAINT [FK_HU_MEAL_RECORD_MSDS_Customer]
GO
ALTER TABLE [dbo].[MembershipUser]  WITH CHECK ADD  CONSTRAINT [FK_MembershipUser_Company] FOREIGN KEY([Company_Id])
REFERENCES [dbo].[Company] ([Id])
GO
ALTER TABLE [dbo].[MembershipUser] CHECK CONSTRAINT [FK_MembershipUser_Company]
GO
ALTER TABLE [dbo].[MembershipUser_MembershipRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_MembershipUser_MembershipRole_Mapping_MembershipRole] FOREIGN KEY([MembershipRole_Id])
REFERENCES [dbo].[MembershipRole] ([Id])
GO
ALTER TABLE [dbo].[MembershipUser_MembershipRole_Mapping] CHECK CONSTRAINT [FK_MembershipUser_MembershipRole_Mapping_MembershipRole]
GO
ALTER TABLE [dbo].[MembershipUser_MembershipRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_MembershipUser_MembershipRole_Mapping_MembershipUser] FOREIGN KEY([MembershipUser_Id])
REFERENCES [dbo].[MembershipUser] ([Id])
GO
ALTER TABLE [dbo].[MembershipUser_MembershipRole_Mapping] CHECK CONSTRAINT [FK_MembershipUser_MembershipRole_Mapping_MembershipUser]
GO
ALTER TABLE [dbo].[MenuItem_MembershipRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_MenuItem_MembershipRole_Mapping_MembershipRole] FOREIGN KEY([MembershipRole_Id])
REFERENCES [dbo].[MembershipRole] ([Id])
GO
ALTER TABLE [dbo].[MenuItem_MembershipRole_Mapping] CHECK CONSTRAINT [FK_MenuItem_MembershipRole_Mapping_MembershipRole]
GO
ALTER TABLE [dbo].[MenuItem_MembershipRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_MenuItem_MembershipRole_Mapping_MenuItem] FOREIGN KEY([MenuItem_Id])
REFERENCES [dbo].[MenuItem] ([Id])
GO
ALTER TABLE [dbo].[MenuItem_MembershipRole_Mapping] CHECK CONSTRAINT [FK_MenuItem_MembershipRole_Mapping_MenuItem]
GO
ALTER TABLE [dbo].[PermissionRecord_MembershipRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_PermissionRecord_MembershipRole_Mapping_MembershipRole] FOREIGN KEY([MembershipRole_Id])
REFERENCES [dbo].[MembershipRole] ([Id])
GO
ALTER TABLE [dbo].[PermissionRecord_MembershipRole_Mapping] CHECK CONSTRAINT [FK_PermissionRecord_MembershipRole_Mapping_MembershipRole]
GO
USE [master]
GO
ALTER DATABASE [Husky] SET  READ_WRITE 
GO
