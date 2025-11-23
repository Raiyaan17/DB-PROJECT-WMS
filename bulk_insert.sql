USE WheresMyScheduleDB;
GO

--- testing ---
-- Delete from Dept.Department;
-- Delete from School.School;
-- Delete from Std.Student;

-- Perform the bulk insert from the CSV file into the Dept.Department table
BULK INSERT Dept.Department
FROM '/var/opt/mssql/Dept.Department.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);
GO

-- SELECT * FROM Dept.Department;
-- GO

BULK INSERT School.School
FROM '/var/opt/mssql/School.School.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);
GO

-- SELECT * FROM School.School;
-- GO

BULK INSERT Std.Student
FROM '/var/opt/mssql/StudentArchiveData.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);
GO

-- SELECT TOP 10 * FROM Std.Student;
GO

BULK INSERT School.Department
FROM '/var/opt/mssql/School.Department.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);
GO

BULK INSERT Course.Course
FROM '/var/opt/mssql/Course.Course.csv'
WITH (
    FORMAT = 'CSV',
    FIRSTROW = 2
);
GO

BULK INSERT Course.CoursePrerequisite
FROM '/var/opt/mssql/Course.CoursePrequisite.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);
GO

