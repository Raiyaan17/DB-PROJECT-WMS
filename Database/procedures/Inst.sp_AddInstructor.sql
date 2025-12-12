USE WheresMyScheduleDB;
GO

CREATE OR ALTER PROCEDURE Inst.sp_AddInstructor
    @InstructorID VARCHAR(30),
    @FName        VARCHAR(50),
    @LName        VARCHAR(50),
    @Email        VARCHAR(100),
    @DepartmentID VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Dept.Department WHERE DepartmentID = @DepartmentID)
        RAISERROR('Department does not exist.', 16, 1);

    IF EXISTS (SELECT 1 FROM Inst.Instructor WHERE InstructorID = @InstructorID)
        RAISERROR('Instructor already exists.', 16, 1);

    INSERT INTO Inst.Instructor (InstructorID, FName, LName, Email, DepartmentID)
    VALUES (@InstructorID, @FName, @LName, @Email, @DepartmentID);
END
GO
