CREATE OR ALTER PROCEDURE Std.sp_ForceEnroll
    @AdminID VARCHAR(30),
    @StudentID VARCHAR(30),
    @CourseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- Ensure the course exists (even if it is inactive or full)
        IF NOT EXISTS (
            SELECT 1
            FROM Course.Course WITH (UPDLOCK, HOLDLOCK)
            WHERE CourseCode = @CourseCode
        )
        BEGIN
            RAISERROR ('Course does not exist.', 16, 1);
        END

        DECLARE @AlreadyEnrolled BIT = 0;

        IF EXISTS (
            SELECT 1 
            FROM Std.Enrollment WITH (UPDLOCK, HOLDLOCK)
            WHERE StudentID = @StudentID AND CourseCode = @CourseCode
        )
        BEGIN
            SET @AlreadyEnrolled = 1;
        END
        ELSE
        BEGIN
            INSERT INTO Std.Enrollment (StudentID, CourseCode, Completed, IsForced)
            VALUES (@StudentID, @CourseCode, 0, 1);
        END

        -- Track seat usage even for forced enrollments (may go negative)
        -- RemainingSeats is now a computed column and is updated automatically
        -- by the trigger on Std.Enrollment updating EnrolledCount.
        -- So, no direct update to RemainingSeats is needed here.

        INSERT INTO Std.AuditLog (AdminID, StudentID, CourseCode, ActionDescription)
        VALUES (@AdminID, @StudentID, @CourseCode, 'Forced enrollment override');

        COMMIT TRAN;
        PRINT 'Force enrollment completed.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
