-- Drop the procedure if it already exists to allow for re-creation
DROP PROCEDURE IF EXISTS Course.usp_AddToWaitlist;
GO

CREATE PROCEDURE Course.usp_AddToWaitlist
    @studentId NVARCHAR(450),
    @courseCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate input
    IF NOT EXISTS (SELECT 1 FROM Std.Student WHERE StudentId = @studentId)
    BEGIN
        RAISERROR('Student does not exist.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Course.Course WHERE CourseCode = @courseCode)
    BEGIN
        RAISERROR('Course does not exist.', 16, 1);
        RETURN;
    END

    -- Check if the course is full
    DECLARE @remainingSeats INT;
    SELECT @remainingSeats = RemainingSeats
    FROM Course.Course
    WHERE CourseCode = @courseCode;

    IF @remainingSeats > 0
    BEGIN
        RAISERROR('This course has open seats. Please enroll directly instead of joining the waitlist.', 16, 1);
        RETURN;
    END

    -- Check if student is already on the waitlist
    IF EXISTS (SELECT 1 FROM Course.Waitlist WHERE StudentId = @studentId AND CourseCode = @courseCode)
    BEGIN
        RAISERROR('You are already on the waitlist for this course.', 16, 1);
        RETURN;
    END

    -- Add the student to the waitlist
    BEGIN TRY
        DECLARE @gradYear SMALLINT;
        SELECT @gradYear = GraduationYear FROM Std.Student WHERE StudentId = @studentId;

        INSERT INTO Course.Waitlist (CourseCode, StudentId, GraduationYear, WaitlistTimestamp)
        VALUES (@courseCode, @studentId, @gradYear, GETUTCDATE());
    END TRY
    BEGIN CATCH
        -- Re-throw any error that occurs during insert
        THROW;
    END CATCH;
END;
GO
