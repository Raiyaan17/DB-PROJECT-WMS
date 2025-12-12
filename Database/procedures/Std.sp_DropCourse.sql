-- Drop the procedure if it already exists to allow for re-creation
DROP PROCEDURE IF EXISTS Std.sp_DropCourse;
GO

CREATE PROCEDURE Std.sp_DropCourse
    @studentId NVARCHAR(450),
    @courseCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
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

        -- Check if the student is actually enrolled in the course
        IF NOT EXISTS (SELECT 1 FROM Std.Enrollment WHERE StudentId = @studentId AND CourseCode = @courseCode)
        BEGIN
            RAISERROR('You are not enrolled in this course.', 16, 1);
            RETURN;
        END

        -- Delete the enrollment record.
        -- A trigger on the Enrollment table will handle updating the course counts.
        DELETE FROM Std.Enrollment
        WHERE StudentId = @studentId AND CourseCode = @courseCode;

    END TRY
    BEGIN CATCH
        -- Re-throw any error that occurs
        THROW;
    END CATCH;
END;
GO
