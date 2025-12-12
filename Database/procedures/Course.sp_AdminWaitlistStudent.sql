USE WheresMyScheduleDB;
GO

CREATE OR ALTER PROCEDURE Course.sp_AdminWaitlistStudent
    @StudentID  VARCHAR(30),
    @CourseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GraduationYear SMALLINT;

    SELECT @GraduationYear = GraduationYear
    FROM Std.Student
    WHERE StudentID = @StudentID;

    IF @GraduationYear IS NULL
        RAISERROR('Student does not exist.', 16, 1);

    IF NOT EXISTS (SELECT 1 FROM Course.Course WHERE CourseCode = @CourseCode)
        RAISERROR('Course does not exist.', 16, 1);

    IF EXISTS (SELECT 1 FROM Std.Enrollment WHERE StudentID = @StudentID AND CourseCode = @CourseCode)
        RAISERROR('Student already enrolled in this course.', 16, 1);

    IF EXISTS (SELECT 1 FROM Course.Waitlist WHERE StudentID = @StudentID AND CourseCode = @CourseCode)
        RAISERROR('Student is already on the waitlist for this course.', 16, 1);

    INSERT INTO Course.Waitlist (CourseCode, StudentID, GraduationYear)
    VALUES (@CourseCode, @StudentID, @GraduationYear);
END
GO
