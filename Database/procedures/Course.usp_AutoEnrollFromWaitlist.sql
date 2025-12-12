USE WheresMyScheduleDB;
GO

-- Drop the procedure if it already exists to allow for re-creation
DROP PROCEDURE IF EXISTS Course.usp_AutoEnrollFromWaitlist;
GO

CREATE PROCEDURE Course.usp_AutoEnrollFromWaitlist
    @CourseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TopStudentID VARCHAR(30);
    DECLARE @StudentGraduationYear SMALLINT;
    DECLARE @RemainingSeats INT;

    BEGIN TRY
        BEGIN TRAN;

        -- Check if there are remaining seats in the course
        SELECT @RemainingSeats = RemainingSeats
        FROM Course.Course WITH (UPDLOCK) -- Use UPDLOCK to prevent race conditions
        WHERE CourseCode = @CourseCode;

        -- If there are no remaining seats, or the course does not exist, do nothing
        IF @RemainingSeats <= 0 OR @RemainingSeats IS NULL
        BEGIN
            COMMIT TRAN;
            RETURN;
        END

        -- Find the top student on the waitlist (FIFO)
        SELECT TOP 1
            @TopStudentID = StudentID,
            @StudentGraduationYear = GraduationYear
        FROM Course.Waitlist WITH (UPDLOCK) -- Use UPDLOCK to prevent race conditions
        WHERE CourseCode = @CourseCode
        ORDER BY WaitlistTimestamp ASC;

        -- If no student is on the waitlist, do nothing
        IF @TopStudentID IS NULL
        BEGIN
            COMMIT TRAN;
            RETURN;
        END

        -- === Prerequisite Check ===
        DECLARE @PrerequisitesMet BIT;
        SELECT @PrerequisitesMet = Course.fn_CheckPrerequisiteStatus(@TopStudentID, @CourseCode);

        IF @PrerequisitesMet = 0
        BEGIN
            PRINT 'Auto-enrollment failed for ' + @TopStudentID + ' into ' + @CourseCode + ': Prerequisites not met.';
            -- Student remains on waitlist.
            COMMIT TRAN;
            RETURN;
        END

        -- === Time Conflict Check ===
        DECLARE @HasTimeConflict BIT;
        SELECT @HasTimeConflict = Course.fn_CheckEnrollmentTimeConflict(@TopStudentID, @CourseCode);

        IF @HasTimeConflict = 1
        BEGIN
            PRINT 'Auto-enrollment failed for ' + @TopStudentID + ' into ' + @CourseCode + ': Time conflict with an already enrolled course.';
            -- Student remains on waitlist.
            COMMIT TRAN;
            RETURN;
        END

        -- Attempt to enroll the student using the existing stored procedure
        -- We need to call Std.sp_EnrollStudent which updates the EnrolledCount
        -- This call will also handle any other enrollment validations (e.g., student already enrolled)
        EXEC Std.sp_EnrollStudent @StudentID = @TopStudentID, @CourseCode = @CourseCode;

        -- If enrollment is successful, remove the student from the waitlist
        DELETE FROM Course.Waitlist
        WHERE CourseCode = @CourseCode AND StudentID = @TopStudentID;

        COMMIT TRAN;
        PRINT 'Auto-enrollment successful for ' + @TopStudentID + ' into ' + @CourseCode;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        -- Log the error or re-throw it if necessary
        PRINT 'Error during auto-enrollment for ' + @TopStudentID + ' into ' + @CourseCode + ': ' + ERROR_MESSAGE();
        -- THROW; -- Re-throw if you want the outer transaction to fail
    END CATCH
END;
GO
