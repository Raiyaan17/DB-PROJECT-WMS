-- =============================================
-- Drop All Database Objects
-- =============================================
-- This script drops all tables, views, procedures, functions, and schemas from the database.
-- It is intended for a complete teardown and rebuild.

USE WheresMyScheduleDB;
GO

PRINT '--- Dropping Triggers ---';
-- Triggers are dropped automatically when their table is dropped,
-- but we drop them explicitly here for a complete script.
DROP TRIGGER IF EXISTS trg_CheckCreditLimit;
DROP TRIGGER IF EXISTS trg_PreventAlumniEnrollment;
DROP TRIGGER IF EXISTS Std.trg_UpdateCourseCounts;
DROP TRIGGER IF EXISTS Inst.trg_GenerateInstructorID;
GO

PRINT '--- Dropping Views ---';
-- Drop temporary views from data loading
DROP VIEW IF EXISTS Course.vCourse_Import;
DROP VIEW IF EXISTS Std.vStudent_Import;
-- Drop regular views
DROP VIEW IF EXISTS Course.vw_AvailableCourses;
DROP VIEW IF EXISTS Std.vw_StudentSchedule;
DROP VIEW IF EXISTS School.vw_AdminDashboardStats;
DROP VIEW IF EXISTS Course.vw_FullPrerequisiteList;
DROP VIEW IF EXISTS Std.vw_StudentTranscript;
DROP VIEW IF EXISTS Analytics.v_DepartmentStudentCounts;
GO

PRINT '--- Dropping Stored Procedures ---';
DROP PROCEDURE IF EXISTS Std.sp_EnrollStudent;
DROP PROCEDURE IF EXISTS Std.sp_ForceEnroll;
DROP PROCEDURE IF EXISTS Course.usp_GetAllPrerequisites;
DROP PROCEDURE IF EXISTS Std.sp_EnrollFromCart;
DROP PROCEDURE IF EXISTS Course.usp_AddToWaitlist;
DROP PROCEDURE IF EXISTS Std.sp_DropCourse;
DROP PROCEDURE IF EXISTS Course.usp_AutoEnrollFromWaitlist;
-- Stored procedures for SProc BLL
DROP PROCEDURE IF EXISTS Std.sp_AddToCart;
DROP PROCEDURE IF EXISTS Std.sp_RemoveFromCart;
DROP PROCEDURE IF EXISTS Std.sp_ValidateCart;
DROP PROCEDURE IF EXISTS Std.sp_GetSchedule;
DROP PROCEDURE IF EXISTS Course.sp_GetAvailableCourses;
DROP PROCEDURE IF EXISTS Course.sp_GetCourseDetails;
DROP PROCEDURE IF EXISTS Std.sp_GetCart;
DROP PROCEDURE IF EXISTS Std.sp_GetStudent;
DROP PROCEDURE IF EXISTS Course.sp_GetDepartments;
DROP PROCEDURE IF EXISTS Course.sp_SearchCourses;
DROP PROCEDURE IF EXISTS Course.sp_AddCourse;
DROP PROCEDURE IF EXISTS Inst.sp_AddInstructor;
DROP PROCEDURE IF EXISTS Std.sp_AddStudent;
DROP PROCEDURE IF EXISTS Course.sp_AdminWaitlistStudent;
DROP PROCEDURE IF EXISTS Course.sp_SetCourseStatus;
DROP PROCEDURE IF EXISTS Std.sp_SetEnrollmentCompletion;
DROP PROCEDURE IF EXISTS Course.sp_UpdateCapacity;
DROP PROCEDURE IF EXISTS sp_Admin_GetStudents;
DROP PROCEDURE IF EXISTS sp_Admin_GetTotalStudentCount;
DROP PROCEDURE IF EXISTS sp_Admin_GetArchivedYears;
DROP PROCEDURE IF EXISTS sp_Admin_GetArchivedStudentsByYear;
DROP PROCEDURE IF EXISTS sp_Admin_GetInstructors;
DROP PROCEDURE IF EXISTS sp_Admin_GetCourses;
DROP PROCEDURE IF EXISTS sp_Admin_GetDepartments;
GO

PRINT '--- Dropping Functions ---';
DROP FUNCTION IF EXISTS Course.fn_GetRemainingSeats;
DROP FUNCTION IF EXISTS Course.fn_CheckPrerequisiteStatus;
DROP FUNCTION IF EXISTS Course.fn_GetCartTimeConflicts;
DROP FUNCTION IF EXISTS Course.fn_CheckEnrollmentTimeConflict;
GO

PRINT '--- Dropping Tables ---';
-- Drop tables in the correct order to respect foreign key constraints
DROP TABLE IF EXISTS Std.CourseCart;
DROP TABLE IF EXISTS Course.Waitlist;
DROP TABLE IF EXISTS Std.Enrollment;
DROP TABLE IF EXISTS Inst.TeachingAssignment;
DROP TABLE IF EXISTS Dept.DegreeCoreCourse;
DROP TABLE IF EXISTS Dept.DegreeElectiveCourse;
DROP TABLE IF EXISTS Course.CoursePrerequisite;
DROP TABLE IF EXISTS Dept.Courses;
DROP TABLE IF EXISTS School.Department;
DROP TABLE IF EXISTS Std.StudentIdSequence;
DROP TABLE IF EXISTS Std.AuditLog;
DROP TABLE IF EXISTS Inst.InstructorIdSequence;
DROP TABLE IF EXISTS Std.Student;
DROP TABLE IF EXISTS Inst.Instructor;
DROP TABLE IF EXISTS Course.Course;
DROP TABLE IF EXISTS Dept.Department;
DROP TABLE IF EXISTS School.Admin;
DROP TABLE IF EXISTS School.School;
GO

PRINT '--- Dropping Schemas ---';
DROP SCHEMA IF EXISTS Std;
DROP SCHEMA IF EXISTS Inst;
DROP SCHEMA IF EXISTS Dept;
DROP SCHEMA IF EXISTS Course;
DROP SCHEMA IF EXISTS School;
DROP SCHEMA IF EXISTS Analytics;
GO

PRINT '--- Dropping Partitioning Objects ---';
IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'ps_GraduatingYear')
    DROP PARTITION SCHEME ps_GraduatingYear;
GO
IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'pf_GraduatingYear')
    DROP PARTITION FUNCTION pf_GraduatingYear;
GO

PRINT '--- Teardown Complete ---';
GO