-- =============================================
-- Master Database Initialization Script
-- =============================================
-- This script orchestrates the entire database setup by executing individual component scripts.
-- It should be run using sqlcmd mode.

:on error exit

PRINT '--- Starting Database Initialization ---';
PRINT 'Step 1: Creating database and setting context...';
USE master;
GO
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WheresMyScheduleDB')
BEGIN
    CREATE DATABASE WheresMyScheduleDB;
END
GO
USE WheresMyScheduleDB;
GO

PRINT 'Step 2: Creating Schemas...';
GO
:r ./schemas.sql
GO

PRINT 'Step 3: Creating Static Tables (Department, School)...';
GO
:r ./tables/Course.sql
:r ./tables/Dept.sql
:r ./tables/School.sql
GO

PRINT 'Step 5: Creating and Executing Student Table Partitioning Setup...';
GO
:r ./procedures/setup_student_partitioning.sql
EXEC dbo.SetupStudentTablePartitioning;
DROP PROCEDURE dbo.SetupStudentTablePartitioning;
GO

PRINT 'Step 6: Creating Helper and Remaining Tables...';
GO
:r ./tables/Std.sql
:r ./tables/Inst.sql
GO

PRINT 'Step 7: Creating Stored Procedures...';
GO
:r ./procedures/Course.usp_GetAllPrerequisites.sql
GO

PRINT 'Step 8: Creating Views...';
GO
:r ./views/Analytics.v_DepartmentStudentCounts.sql
GO

PRINT '--- Database Initialization Complete ---';
GO
