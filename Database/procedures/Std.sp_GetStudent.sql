USE WheresMyScheduleDB;
GO

-- 2. Get Student Details
CREATE OR ALTER PROCEDURE Std.sp_GetStudent
    @studentId VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        s.StudentId,
        s.Fname,
        s.Lname,
        s.Email,
        s.DepartmentId,
        s.GraduationYear,
        sch.SchoolName
    FROM Std.Student s
    JOIN School.School sch ON s.SchoolID = sch.SchoolID
    WHERE s.StudentId = @studentId;
END;
GO
