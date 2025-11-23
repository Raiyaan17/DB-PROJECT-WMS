USE WheresMyScheduleDB;
GO


BULK INSERT Dept.Department
FROM '/var/opt/mssql/Dept.Department.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);
GO


BULK INSERT School.School
FROM '/var/opt/mssql/School.School.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);
GO


BULK INSERT Std.Student
FROM '/var/opt/mssql/StudentArchiveData.csv'
WITH (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);
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


BULK INSERT Dept.Courses
FROM '/var/opt/mssql/Dept.Courses.csv'
WITH (
    FORMAT = 'CSV',
    FIRSTROW = 2
);
GO

BULK INSERT Inst.Instructor
FROM '/var/opt/mssql/Inst.Instructor.csv'
WITH (
    FORMAT = 'CSV',
    FIRSTROW = 2
);
GO


BULK INSERT Inst.TeachingAssignment
FROM '/var/opt/mssql/Inst.TeachingAssignment.csv'
WITH (
    FORMAT = 'CSV',
    FIRSTROW = 2
);
GO


-- Verification Section -- 
-- SELECT * FROM Dept.Department;
-- GO

-- SELECT * FROM School.School;
-- GO

-- SELECT * FROM Std.Student;
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