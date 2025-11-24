USE WheresMyScheduleDB;
GO

----------------------------------------------------------
-- STORED PROCEDURE 1: Standard Student Enrollment
-- Executes all normal checks: no of seats / duplicate enrollment
-- Inserts enrollment + reduces RemainingSeats
-----------------------------------------------------------
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

        UPDATE Course.Course
        SET RemainingSeats = RemainingSeats - 1
        WHERE CourseCode = @CourseCode;

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

-----------------------------------------------------------
-- STORED PROCEDURE 2: Admin Forced Enrollment
-- Bypasses all constraints
-- Inserts enrollment (if not already)
-- Logs action into Std.AuditLog
-----------------------------------------------------------
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
        IF @AlreadyEnrolled = 0
        BEGIN
            UPDATE Course.Course
            SET RemainingSeats = RemainingSeats - 1
            WHERE CourseCode = @CourseCode;
        END

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
