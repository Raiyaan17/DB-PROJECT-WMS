USE WheresMyScheduleDB;
GO

CREATE OR ALTER PROCEDURE Std.sp_ForceEnroll
    @StudentID  VARCHAR(30),
    @CourseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @GraduationYear SMALLINT;

        SELECT @GraduationYear = GraduationYear
        FROM Std.Student WITH (UPDLOCK, HOLDLOCK)
        WHERE StudentID = @StudentID;

        IF @GraduationYear IS NULL
            RAISERROR('Student does not exist.', 16, 1);

        IF NOT EXISTS (
            SELECT 1 FROM Course.Course WITH (UPDLOCK, HOLDLOCK)
            WHERE CourseCode = @CourseCode
        )
            RAISERROR('Course does not exist.', 16, 1);

        IF NOT EXISTS (
            SELECT 1 FROM Course.Waitlist WITH (UPDLOCK, HOLDLOCK)
            WHERE StudentID = @StudentID AND CourseCode = @CourseCode
        )
            RAISERROR('Student is not on the waitlist for this course.', 16, 1);

        IF EXISTS (
            SELECT 1 FROM Std.Enrollment WITH (UPDLOCK, HOLDLOCK)
            WHERE StudentID = @StudentID AND CourseCode = @CourseCode
        )
            RAISERROR('Student already enrolled in this course.', 16, 1);

        INSERT INTO Std.Enrollment (StudentID, GraduationYear, CourseCode, Completed, IsForced)
        VALUES (@StudentID, @GraduationYear, @CourseCode, 0, 1);

        DELETE FROM Course.Waitlist
        WHERE StudentID = @StudentID AND CourseCode = @CourseCode;

        INSERT INTO Std.AuditLog (AdminID, StudentID, CourseCode, ActionDescription)
        VALUES (NULL, @StudentID, @CourseCode, 'Forced enrollment from waitlist');

        COMMIT TRAN;
        PRINT 'Force enrollment from waitlist completed.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
