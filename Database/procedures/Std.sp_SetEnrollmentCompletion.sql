USE WheresMyScheduleDB;
GO

CREATE OR ALTER PROCEDURE Std.sp_SetEnrollmentCompletion
    @StudentID  VARCHAR(30),
    @CourseCode VARCHAR(10),
    @Completed  BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Std.Enrollment WHERE StudentID = @StudentID AND CourseCode = @CourseCode)
        RAISERROR('Enrollment does not exist for the given student and course.', 16, 1);

    UPDATE Std.Enrollment
    SET Completed = @Completed
    WHERE StudentID = @StudentID
      AND CourseCode = @CourseCode;
END
GO
