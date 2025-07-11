USE [master]
GO
/****** Object:  Database [PlanyDB]    Script Date: 6/7/2025 10:07:29 AM ******/
CREATE DATABASE [PlanyDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PlanyDB', FILENAME = N'/var/opt/mssql/data/PlanyDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PlanyDB_log', FILENAME = N'/var/opt/mssql/data/PlanyDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [PlanyDB] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PlanyDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PlanyDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PlanyDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PlanyDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PlanyDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PlanyDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [PlanyDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PlanyDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PlanyDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PlanyDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PlanyDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PlanyDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PlanyDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PlanyDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PlanyDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PlanyDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PlanyDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PlanyDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PlanyDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PlanyDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PlanyDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PlanyDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PlanyDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PlanyDB] SET RECOVERY FULL 
GO
ALTER DATABASE [PlanyDB] SET  MULTI_USER 
GO
ALTER DATABASE [PlanyDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PlanyDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PlanyDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PlanyDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PlanyDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PlanyDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'PlanyDB', N'ON'
GO
ALTER DATABASE [PlanyDB] SET QUERY_STORE = OFF
GO
USE [PlanyDB]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Challenges]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Challenges](
	[ChallengeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ItemID] [int] NULL,
	[PackageID] [int] NULL,
	[PointsAwarded] [int] NULL,
	[DifficultyLevel] [nvarchar](50) NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[EndTime] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedByUserID] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ChallengeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GroupMembers]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupMembers](
	[GroupID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[CashContributed] [decimal](18, 2) NULL,
	[JoinedAt] [datetime2](7) NOT NULL,
	[RoleInGroup] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Groups](
	[GroupID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[PlanID] [int] NULL,
	[GroupStart] [datetime2](7) NOT NULL,
	[GroupEnd] [datetime2](7) NOT NULL,
	[TotalMember] [int] NOT NULL,
	[OwnerID] [int] NOT NULL,
	[IsPrivate] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hotels]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hotels](
	[ItemID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](max) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[CheckInTime] [time](7) NULL,
	[CheckOutTime] [time](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Images]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Images](
	[ImageID] [int] IDENTITY(1,1) NOT NULL,
	[ReferenceType] [nvarchar](50) NOT NULL,
	[ReferenceID] [int] NOT NULL,
	[ImageData] [varbinary](max) NOT NULL,
	[ContentType] [nvarchar](100) NULL,
	[FileSizeKB] [int] NULL,
	[IsPrimary] [bit] NULL,
	[Caption] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[PackageID] [int] NULL,
	[InvoiceDate] [datetime2](7) NOT NULL,
	[DueDate] [datetime2](7) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Discount] [decimal](18, 2) NOT NULL,
	[FinalAmount]  AS ([Amount]-[Discount]),
	[Status] [nvarchar](50) NOT NULL,
	[PaymentMethod] [nvarchar](100) NULL,
	[TransactionID] [nvarchar](255) NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Items]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Items](
	[ItemID] [int] IDENTITY(1,1) NOT NULL,
	[ItemType] [nvarchar](50) NOT NULL,
	[CategoryID] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Packages]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Packages](
	[PackageID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DurationDays] [int] NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PackageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Places]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Places](
	[ItemID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](max) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[Price] [decimal](18, 2) NULL,
	[Description] [nvarchar](max) NULL,
	[OpenTime] [time](7) NULL,
	[CloseTime] [time](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanList]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanList](
	[PlanListID] [int] IDENTITY(1,1) NOT NULL,
	[PlanID] [int] NOT NULL,
	[ItemNo] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[DayNumber] [int] NOT NULL,
	[StartTime] [time](7) NULL,
	[EndTime] [time](7) NULL,
	[Notes] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[PlanListID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plans]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plans](
	[PlanID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DayCount] [int] NOT NULL,
	[NightCount] [int] NOT NULL,
	[TotalCost] [decimal](18, 2) NOT NULL,
	[OwnerID] [int] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[IsPublic] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ratings]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ratings](
	[RatingID] [int] IDENTITY(1,1) NOT NULL,
	[PlanID] [int] NULL,
	[ItemID] [int] NULL,
	[UserID] [int] NOT NULL,
	[Rate] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[DatePosted] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transportations]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transportations](
	[ItemID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](max) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[OpenTime] [time](7) NULL,
	[CloseTime] [time](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserChallengeProgress]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserChallengeProgress](
	[UserChallengeProgressID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ChallengeID] [int] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[StartedAt] [datetime2](7) NULL,
	[CompletedAt] [datetime2](7) NULL,
	[ProofImageID] [int] NULL,
	[VerificationNotes] [nvarchar](max) NULL,
	[PointsEarned] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserChallengeProgressID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 6/7/2025 10:07:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](15) NULL,
	[Address] [nvarchar](255) NULL,
	[Avatar] [nvarchar](255) NULL,
	[RoleId] [int] NULL,
	[GoogleId] [nvarchar](255) NULL,
	[EmailVerified] [bit] NOT NULL,
	[PasswordResetToken] [nvarchar](255) NULL,
	[PasswordResetTokenExpiresAt] [datetime2](7) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[EmailVerificationToken] [nvarchar](4) NULL,
	[EmailVerificationTokenExpiresAt] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [PlanyDB] SET  READ_WRITE 
GO
