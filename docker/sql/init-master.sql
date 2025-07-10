-- Create Master Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StoockerMT_Master')
BEGIN
    CREATE DATABASE StoockerMT_Master;
END
GO

USE StoockerMT_Master;
GO

-- Create login for application
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'stoockermt_app')
BEGIN
    CREATE LOGIN stoockermt_app WITH PASSWORD = 'App@Password123';
END
GO

-- Create user and grant permissions
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'stoockermt_app')
BEGIN
    CREATE USER stoockermt_app FOR LOGIN stoockermt_app;
    ALTER ROLE db_owner ADD MEMBER stoockermt_app;
END
GO

-- Enable snapshot isolation
ALTER DATABASE StoockerMT_Master SET ALLOW_SNAPSHOT_ISOLATION ON;
ALTER DATABASE StoockerMT_Master SET READ_COMMITTED_SNAPSHOT ON;
GO

-- Create performance indexes on system tables
CREATE INDEX IX_sysobjects_name ON sysobjects(name);
GO

-- Enable Query Store
ALTER DATABASE StoockerMT_Master SET QUERY_STORE = ON;
GO