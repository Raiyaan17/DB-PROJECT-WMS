USE WheresMyScheduleDB;
GO

CREATE OR ALTER PROCEDURE Std.sp_AddStudent
    @StudentID           VARCHAR(30),
    @FName               VARCHAR(50),
    @LName               VARCHAR(50),
    @Email               VARCHAR(100),
    @SchoolID            VARCHAR(30),
    @DepartmentID        VARCHAR(30),
    @GraduationYear      SMALLINT,
    @CurrentAcademicYear VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM School.School WHERE SchoolID = @SchoolID)
        RAISERROR('School does not exist.', 16, 1);

    IF NOT EXISTS (SELECT 1 FROM Dept.Department WHERE DepartmentID = @DepartmentID)
        RAISERROR('Department does not exist.', 16, 1);

    IF EXISTS (SELECT 1 FROM Std.Student WHERE StudentID = @StudentID)
        RAISERROR('Student already exists.', 16, 1);

    INSERT INTO Std.Student (StudentID, FName, LName, Email, SchoolID, DepartmentID, GraduationYear, CurrentAcademicYear)
    VALUES (@StudentID, @FName, @LName, @Email, @SchoolID, @DepartmentID, @GraduationYear, @CurrentAcademicYear);
END
GO
