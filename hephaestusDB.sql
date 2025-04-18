USE [master]
GO
/****** Object:  Database [Hephaestus]    Script Date: 13/04/2025 18:23:55 ******/
CREATE DATABASE [Hephaestus]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Hephaestus', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.GUIVIAN\MSSQL\DATA\Hephaestus.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Hephaestus_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.GUIVIAN\MSSQL\DATA\Hephaestus_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Hephaestus] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Hephaestus].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Hephaestus] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Hephaestus] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Hephaestus] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Hephaestus] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Hephaestus] SET ARITHABORT OFF 
GO
ALTER DATABASE [Hephaestus] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Hephaestus] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Hephaestus] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Hephaestus] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Hephaestus] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Hephaestus] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Hephaestus] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Hephaestus] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Hephaestus] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Hephaestus] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Hephaestus] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Hephaestus] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Hephaestus] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Hephaestus] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Hephaestus] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Hephaestus] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Hephaestus] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Hephaestus] SET RECOVERY FULL 
GO
ALTER DATABASE [Hephaestus] SET  MULTI_USER 
GO
ALTER DATABASE [Hephaestus] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Hephaestus] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Hephaestus] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Hephaestus] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Hephaestus] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Hephaestus] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Hephaestus', N'ON'
GO
ALTER DATABASE [Hephaestus] SET QUERY_STORE = ON
GO
ALTER DATABASE [Hephaestus] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Hephaestus]
GO
/****** Object:  Table [dbo].[tb_anexos]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_anexos](
	[id_anexo] [int] IDENTITY(1,1) NOT NULL,
	[id_ticket] [int] NULL,
	[id_tarefa] [int] NULL,
	[conteudo] [varbinary](max) NOT NULL,
	[conteudo_ct] [varchar](50) NOT NULL,
	[obs] [varchar](100) NULL,
 CONSTRAINT [PK_tb_anexos] PRIMARY KEY CLUSTERED 
(
	[id_anexo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_estado_tarefa]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_estado_tarefa](
	[id_estado] [int] IDENTITY(1,1) NOT NULL,
	[estado] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tb_estado_tarefa] PRIMARY KEY CLUSTERED 
(
	[id_estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_estado_ticket]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_estado_ticket](
	[id_estado] [int] NOT NULL,
	[estado] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tb_estado_ticket] PRIMARY KEY CLUSTERED 
(
	[id_estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_historico_tarefa]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_historico_tarefa](
	[id_historico] [int] IDENTITY(1,1) NOT NULL,
	[id_tarefa] [int] NOT NULL,
	[data_mudanca] [datetime] NOT NULL,
	[descricao] [varchar](255) NOT NULL,
	[id_utilizador] [int] NOT NULL,
 CONSTRAINT [PK_tb_historico_tarefa] PRIMARY KEY CLUSTERED 
(
	[id_historico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_historico_ticket]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_historico_ticket](
	[id_historico] [int] IDENTITY(1,1) NOT NULL,
	[id_ticket] [int] NOT NULL,
	[data_mudanca] [datetime] NOT NULL,
	[descricao] [varchar](255) NOT NULL,
	[id_utilizador] [int] NOT NULL,
 CONSTRAINT [PK_tb_historico_ticket] PRIMARY KEY CLUSTERED 
(
	[id_historico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_prioridade]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_prioridade](
	[id_prioridade] [int] NOT NULL,
	[descricao_suporte] [varchar](100) NOT NULL,
	[descricao_servico] [varchar](100) NOT NULL,
	[descricao_tecnico] [varchar](2) NOT NULL,
 CONSTRAINT [PK_tb_prioridade] PRIMARY KEY CLUSTERED 
(
	[id_prioridade] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_tarefas]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_tarefas](
	[id_tarefa] [int] IDENTITY(1,1) NOT NULL,
	[id_ticket] [int] NOT NULL,
	[id_estado] [int] NOT NULL,
	[id_utilizador] [int] NOT NULL,
	[data_criado] [datetime] NOT NULL,
	[id_historico] [int] NOT NULL,
	[previsao_inicio] [datetime] NULL,
	[previsao_fim] [datetime] NULL,
	[inicio_atual] [datetime] NULL,
	[fim_atual] [datetime] NULL,
	[morada] [varchar](255) NULL,
	[fecho] [varchar](max) NULL,
 CONSTRAINT [PK_tb_tarefas] PRIMARY KEY CLUSTERED 
(
	[id_tarefa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_tickets]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_tickets](
	[id_ticket] [int] IDENTITY(1,1) NOT NULL,
	[id_tipo_ticket] [int] NOT NULL,
	[id_prioridade] [int] NOT NULL,
	[id_estado] [int] NOT NULL,
	[id_utilizador_padrao] [int] NOT NULL,
	[id_utilizador_tecnico] [int] NOT NULL,
	[id_historico] [int] NOT NULL,
	[data_criado] [datetime] NOT NULL,
	[enviar_email] [bit] NOT NULL,
	[morada] [varchar](255) NULL,
	[fecho] [varchar](max) NULL,
 CONSTRAINT [PK_tb_tickets] PRIMARY KEY CLUSTERED 
(
	[id_ticket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_tipo_ticket]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_tipo_ticket](
	[id_tipo] [int] IDENTITY(1,1) NOT NULL,
	[tipo] [varchar](255) NOT NULL,
 CONSTRAINT [PK_tb_tipo_ticket] PRIMARY KEY CLUSTERED 
(
	[id_tipo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_tipo_utilizador]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_tipo_utilizador](
	[id_tipo] [int] IDENTITY(1,1) NOT NULL,
	[tipo] [varchar](255) NOT NULL,
 CONSTRAINT [PK_tb_tipo_utilizador] PRIMARY KEY CLUSTERED 
(
	[id_tipo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tb_utilizadores]    Script Date: 13/04/2025 18:23:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tb_utilizadores](
	[id_utilizador] [int] IDENTITY(1,1) NOT NULL,
	[nome] [varchar](255) NOT NULL,
	[email] [varchar](255) NOT NULL,
	[palavra_passe] [varchar](255) NOT NULL,
	[tlm] [varchar](25) NULL,
	[id_tipo] [int] NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_tb_utilizadores] PRIMARY KEY CLUSTERED 
(
	[id_utilizador] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tb_historico_tarefa] ADD  CONSTRAINT [DF_tb_historico_tarefa_data_mudanca]  DEFAULT (getdate()) FOR [data_mudanca]
GO
ALTER TABLE [dbo].[tb_historico_ticket] ADD  CONSTRAINT [DF_tb_historico_ticket_data_mudanca]  DEFAULT (getdate()) FOR [data_mudanca]
GO
ALTER TABLE [dbo].[tb_tarefas] ADD  CONSTRAINT [DF_tb_tarefas_data_criado]  DEFAULT (getdate()) FOR [data_criado]
GO
ALTER TABLE [dbo].[tb_tickets] ADD  CONSTRAINT [DF_tb_tickets_data_criado]  DEFAULT (getdate()) FOR [data_criado]
GO
ALTER TABLE [dbo].[tb_tickets] ADD  CONSTRAINT [DF_tb_tickets_enviar_email]  DEFAULT ((1)) FOR [enviar_email]
GO
ALTER TABLE [dbo].[tb_utilizadores] ADD  CONSTRAINT [DF_tb_utilizadores_ativo]  DEFAULT ((0)) FOR [ativo]
GO
ALTER TABLE [dbo].[tb_anexos]  WITH CHECK ADD  CONSTRAINT [FK_tb_anexos_tb_tarefas] FOREIGN KEY([id_tarefa])
REFERENCES [dbo].[tb_tarefas] ([id_tarefa])
GO
ALTER TABLE [dbo].[tb_anexos] CHECK CONSTRAINT [FK_tb_anexos_tb_tarefas]
GO
ALTER TABLE [dbo].[tb_anexos]  WITH CHECK ADD  CONSTRAINT [FK_tb_anexos_tb_tickets] FOREIGN KEY([id_ticket])
REFERENCES [dbo].[tb_tickets] ([id_ticket])
GO
ALTER TABLE [dbo].[tb_anexos] CHECK CONSTRAINT [FK_tb_anexos_tb_tickets]
GO
ALTER TABLE [dbo].[tb_tarefas]  WITH CHECK ADD  CONSTRAINT [FK_tb_tarefas_tb_estado_tarefa] FOREIGN KEY([id_estado])
REFERENCES [dbo].[tb_estado_tarefa] ([id_estado])
GO
ALTER TABLE [dbo].[tb_tarefas] CHECK CONSTRAINT [FK_tb_tarefas_tb_estado_tarefa]
GO
ALTER TABLE [dbo].[tb_tarefas]  WITH CHECK ADD  CONSTRAINT [FK_tb_tarefas_tb_historico_tarefa] FOREIGN KEY([id_historico])
REFERENCES [dbo].[tb_historico_tarefa] ([id_historico])
GO
ALTER TABLE [dbo].[tb_tarefas] CHECK CONSTRAINT [FK_tb_tarefas_tb_historico_tarefa]
GO
ALTER TABLE [dbo].[tb_tarefas]  WITH CHECK ADD  CONSTRAINT [FK_tb_tarefas_tb_tickets] FOREIGN KEY([id_ticket])
REFERENCES [dbo].[tb_tickets] ([id_ticket])
GO
ALTER TABLE [dbo].[tb_tarefas] CHECK CONSTRAINT [FK_tb_tarefas_tb_tickets]
GO
ALTER TABLE [dbo].[tb_tarefas]  WITH CHECK ADD  CONSTRAINT [FK_tb_tarefas_tb_utilizadores] FOREIGN KEY([id_utilizador])
REFERENCES [dbo].[tb_utilizadores] ([id_utilizador])
GO
ALTER TABLE [dbo].[tb_tarefas] CHECK CONSTRAINT [FK_tb_tarefas_tb_utilizadores]
GO
ALTER TABLE [dbo].[tb_tickets]  WITH CHECK ADD  CONSTRAINT [FK_tb_tickets_tb_estado_ticket] FOREIGN KEY([id_estado])
REFERENCES [dbo].[tb_estado_ticket] ([id_estado])
GO
ALTER TABLE [dbo].[tb_tickets] CHECK CONSTRAINT [FK_tb_tickets_tb_estado_ticket]
GO
ALTER TABLE [dbo].[tb_tickets]  WITH CHECK ADD  CONSTRAINT [FK_tb_tickets_tb_historico_ticket] FOREIGN KEY([id_historico])
REFERENCES [dbo].[tb_historico_ticket] ([id_historico])
GO
ALTER TABLE [dbo].[tb_tickets] CHECK CONSTRAINT [FK_tb_tickets_tb_historico_ticket]
GO
ALTER TABLE [dbo].[tb_tickets]  WITH CHECK ADD  CONSTRAINT [FK_tb_tickets_tb_prioridade] FOREIGN KEY([id_prioridade])
REFERENCES [dbo].[tb_prioridade] ([id_prioridade])
GO
ALTER TABLE [dbo].[tb_tickets] CHECK CONSTRAINT [FK_tb_tickets_tb_prioridade]
GO
ALTER TABLE [dbo].[tb_tickets]  WITH CHECK ADD  CONSTRAINT [FK_tb_tickets_tb_tipo_ticket] FOREIGN KEY([id_tipo_ticket])
REFERENCES [dbo].[tb_tipo_ticket] ([id_tipo])
GO
ALTER TABLE [dbo].[tb_tickets] CHECK CONSTRAINT [FK_tb_tickets_tb_tipo_ticket]
GO
ALTER TABLE [dbo].[tb_tickets]  WITH CHECK ADD  CONSTRAINT [FK_tb_tickets_tb_utilizadores] FOREIGN KEY([id_utilizador_padrao])
REFERENCES [dbo].[tb_utilizadores] ([id_utilizador])
GO
ALTER TABLE [dbo].[tb_tickets] CHECK CONSTRAINT [FK_tb_tickets_tb_utilizadores]
GO
ALTER TABLE [dbo].[tb_tickets]  WITH CHECK ADD  CONSTRAINT [FK_tb_tickets_tb_utilizadores1] FOREIGN KEY([id_utilizador_tecnico])
REFERENCES [dbo].[tb_utilizadores] ([id_utilizador])
GO
ALTER TABLE [dbo].[tb_tickets] CHECK CONSTRAINT [FK_tb_tickets_tb_utilizadores1]
GO
ALTER TABLE [dbo].[tb_utilizadores]  WITH CHECK ADD  CONSTRAINT [FK_tb_utilizadores_tb_tipo_utilizador] FOREIGN KEY([id_tipo])
REFERENCES [dbo].[tb_tipo_utilizador] ([id_tipo])
GO
ALTER TABLE [dbo].[tb_utilizadores] CHECK CONSTRAINT [FK_tb_utilizadores_tb_tipo_utilizador]
GO
USE [master]
GO
ALTER DATABASE [Hephaestus] SET  READ_WRITE 
GO
