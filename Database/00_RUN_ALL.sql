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
:r ./schemas/00_schemas.sql
GO

PRINT 'Step 3: Creating Tables...';
-- Core tables
:r ./tables/School.School.sql
:r ./tables/School.Admin.sql
:r ./tables/Dept.Department.sql
:r ./tables/Course.Course.sql
-- Dependent tables
:r ./tables/School.Department.sql
:r ./tables/Dept.Courses.sql
:r ./tables/Inst.Instructor.sql
:r ./tables/Inst.InstructorIdSequence.sql
:r ./tables/Std.Student.sql
:r ./tables/Std.StudentIdSequence.sql
:r ./tables/Std.AuditLog.sql
:r ./tables/Course.CoursePrerequisite.sql
:r ./tables/Dept.DegreeCoreCourse.sql
:r ./tables/Dept.DegreeElectiveCourse.sql
:r ./tables/Inst.TeachingAssignment.sql
:r ./tables/Std.CourseCart.sql
:r ./tables/Course.Waitlist.sql
:r ./tables/Std.Enrollment.sql
GO

PRINT 'Step 4: Creating Functions...';
GO
:r ./functions/Course.fn_GetRemainingSeats.sql
:r ./functions/Course.fn_CheckPrerequisiteStatus.sql
:r ./functions/Course.fn_GetCartTimeConflicts.sql
:r ./functions/Course.fn_CheckEnrollmentTimeConflict.sql
GO

PRINT 'Step 5: Creating Views...';
GO
:r ./views/Analytics.v_DepartmentStudentCounts.sql
:r ./views/Course.vw_AvailableCourses.sql
:r ./views/Course.vw_FullPrerequisiteList.sql
:r ./views/School.vw_AdminDashboardStats.sql
:r ./views/Std.vw_StudentSchedule.sql
:r ./views/Std.vw_StudentTranscript.sql
GO

PRINT 'Step 6: Creating Stored Procedures...';
GO
:r ./procedures/Course.usp_GetAllPrerequisites.sql
:r ./procedures/Std.sp_EnrollStudent.sql
:r ./procedures/Std.sp_ForceEnroll.sql
:r ./procedures/Std.sp_EnrollFromCart.sql
:r ./procedures/Course.usp_AddToWaitlist.sql
:r ./procedures/Std.sp_DropCourse.sql
:r ./procedures/Course.usp_AutoEnrollFromWaitlist.sql
-- Stored procedures for SProc BLL
:r ./procedures/Std.sp_AddToCart.sql
:r ./procedures/Std.sp_RemoveFromCart.sql
:r ./procedures/Std.sp_ValidateCart.sql
:r ./procedures/Std.sp_GetSchedule.sql
:r ./procedures/Course.sp_GetAvailableCourses.sql
:r ./procedures/Course.sp_GetCourseDetails.sql
:r ./procedures/Std.sp_GetCart.sql
:r ./procedures/Std.sp_GetStudent.sql
:r ./procedures/Course.sp_GetDepartments.sql
:r ./procedures/Course.sp_SearchCourses.sql
GO

PRINT 'Step 7: Creating Triggers...';
GO
:r ./triggers/trg_CheckCreditLimit.sql
:r ./triggers/trg_PreventAlumniEnrollment.sql
:r ./triggers/trg_UpdateCourseCounts.sql
GO

PRINT 'Step 8: Bulk Inserting Data...';
:r ./data/load_data.sql
GO

PRINT '--- Database Initialization Complete ---';
GO
