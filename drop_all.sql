-- Drop tables in the correct order to respect foreign key constraints
-- Tables that reference others are dropped first.
DROP TABLE IF EXISTS Std.Enrollment;
DROP TABLE IF EXISTS Inst.TeachingAssignment;
DROP TABLE IF EXISTS Course.CoursePrerequisite;
DROP TABLE IF EXISTS Dept.DegreeCoreCourse;
DROP TABLE IF EXISTS Dept.DegreeElectiveCourse;
DROP TABLE IF EXISTS Dept.Courses;
DROP TABLE IF EXISTS School.Department;
DROP TABLE IF EXISTS Std.Student;
DROP TABLE IF EXISTS Std.StudentIdSequence;
DROP TABLE IF EXISTS Inst.Instructor;
DROP TABLE IF EXISTS Inst.InstructorIdSequence;
GO

-- Now the tables that were referenced by the ones above
DROP TABLE IF EXISTS Course.Course;
DROP TABLE IF EXISTS Dept.Department;
DROP TABLE IF EXISTS School.School;
GO

-- Drop the schemas
DROP SCHEMA IF EXISTS Std;
DROP SCHEMA IF EXISTS Inst;
DROP SCHEMA IF EXISTS Dept;
DROP SCHEMA IF EXISTS Course;
DROP SCHEMA IF EXISTS School;
GO