USE WheresMyScheduleDB;
GO


BULK INSERT Dept.Department
FROM '/var/opt/mssql/Dept.Department.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    FIELDQUOTE = '"',
    TABLOCK
);
GO


BULK INSERT School.School
FROM '/var/opt/mssql/School.School.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    FIELDQUOTE = '"',
    TABLOCK
);
GO

------------------------------
-- 1. Create a temporary view mapping to the CSV columns (omitting PasswordHash)
CREATE VIEW Std.vStudent_Import AS
SELECT
    StudentID,
    FName,
    LName,
    Email,
    SchoolID,
    DepartmentID,
    GraduationYear,
    CurrentAcademicYear
FROM Std.Student;
GO

-- 2. Bulk insert into the view
BULK INSERT Std.vStudent_Import
FROM '/var/opt/mssql/StudentArchiveData.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    FIELDQUOTE = '"',
    TABLOCK
);
GO

-- 3. Drop the temporary view
DROP VIEW Std.vStudent_Import;
GO
------------------------------

BULK INSERT School.Department
FROM '/var/opt/mssql/School.Department.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    FIELDQUOTE = '"',
    TABLOCK
);
GO

------------------------------
CREATE OR ALTER VIEW Course.vCourse_Import AS
SELECT 
    CourseCode, 
    CourseTitle, 
    TotalCredits, 
    Capacity,
    DayOfWeek,
    StartTime,
    EndTime
FROM Course.Course;
GO

-- 2. Bulk Insert into the VIEW (not the table)
BULK INSERT Course.vCourse_Import
FROM '/var/opt/mssql/Course.Course.csv'
WITH (
    FORMAT = 'CSV',           -- Keeps strict CSV rules for quotes
    FIRSTROW = 2,
    FIELDQUOTE = '"',
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',   -- Matches your dos2unix converted file
    TABLOCK
);
GO

-- 3. Cleanup
DROP VIEW Course.vCourse_Import;
GO
------------------------------

BULK INSERT Course.CoursePrerequisite
FROM '/var/opt/mssql/Course.CoursePrequisite.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    FIELDQUOTE = '"',
    TABLOCK
);
GO


BULK INSERT Dept.Courses
FROM '/var/opt/mssql/Dept.Courses.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    FIELDQUOTE = '"',
    TABLOCK
);
GO

BULK INSERT Inst.Instructor
FROM '/var/opt/mssql/Inst.Instructor.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\r\n',
    FIELDQUOTE = '"',
    TABLOCK
);
GO


BULK INSERT Inst.TeachingAssignment
FROM '/var/opt/mssql/Inst.TeachingAssignment.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    FIELDQUOTE = '"',
    TABLOCK
);
GO


-- Verification Section -- 
-- SELECT * FROM Dept.Department;
-- GO

-- SELECT * FROM School.School;
-- GO

-- SELECT *
-- FROM Std.Student
-- WHERE GraduationYear > 1995 and SchoolID = 'SDSB';
-- GO

-- SELECT * FROM School.Department;
-- GO

-- SELECT * FROM Course.Course;
-- GO

-- SELECT * FROM Course.CoursePrerequisite;
-- GO

-- SELECT * FROM Dept.Courses;
-- GO

-- SELECT * FROM Inst.Instructor;
-- GO

-- SELECT * FROM Inst.TeachingAssignment;
-- GO