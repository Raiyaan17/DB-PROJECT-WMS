CREATE OR ALTER PROCEDURE Std.sp_EnrollStudent
    @StudentID VARCHAR(30),
    @CourseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @RemainingSeats INT;

        -- Lock the target course row while evaluating seats to avoid race conditions
        SELECT @RemainingSeats = RemainingSeats
        FROM Course.Course WITH (UPDLOCK, HOLDLOCK)
        WHERE CourseCode = @CourseCode
          AND IsActive = 1;

        IF @RemainingSeats IS NULL
        BEGIN
            RAISERROR ('Course does not exist or is inactive.', 16, 1);
        END

        IF (@RemainingSeats <= 0)
        BEGIN
            RAISERROR ('Course is full.', 16, 1);
        END

        -- Prevent double enrollment within the same transaction scope
        IF EXISTS (
            SELECT 1 
            FROM Std.Enrollment WITH (UPDLOCK, HOLDLOCK)
            WHERE StudentID = @StudentID AND CourseCode = @CourseCode
        )
        BEGIN
            RAISERROR ('Student already enrolled in this course.', 16, 1);
        END

        INSERT INTO Std.Enrollment (StudentID, CourseCode, Completed, IsForced)
        VALUES (@StudentID, @CourseCode, 0, 0);

        COMMIT TRAN;
        PRINT 'Enrollment successful.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
