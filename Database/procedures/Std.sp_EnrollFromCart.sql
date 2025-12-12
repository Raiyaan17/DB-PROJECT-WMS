-- Drop the procedure if it already exists to allow for re-creation
DROP PROCEDURE IF EXISTS Std.sp_EnrollFromCart;
GO

CREATE PROCEDURE Std.sp_EnrollFromCart
    @studentId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Step 1: Declare temporary tables to categorize courses
    DECLARE @CoursesToEnroll TABLE (CourseCode NVARCHAR(50) PRIMARY KEY);
    DECLARE @CoursesToWaitlist TABLE (CourseCode NVARCHAR(50) PRIMARY KEY);
    DECLARE @FailedCourses TABLE (CourseCode NVARCHAR(50) PRIMARY KEY, ErrorMessage NVARCHAR(255));

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Step 2: Populate initial categories based on course capacity
        INSERT INTO @CoursesToEnroll (CourseCode)
        SELECT cc.CourseCode
        FROM Std.CourseCart cc
        JOIN Course.Course c ON cc.CourseCode = c.CourseCode
        WHERE cc.StudentId = @studentId AND c.RemainingSeats > 0;

        INSERT INTO @CoursesToWaitlist (CourseCode)
        SELECT cc.CourseCode
        FROM Std.CourseCart cc
        JOIN Course.Course c ON cc.CourseCode = c.CourseCode
        WHERE cc.StudentId = @studentId AND c.RemainingSeats <= 0;

        -- Check if the cart was empty to begin with
        IF NOT EXISTS (SELECT 1 FROM @CoursesToEnroll) AND NOT EXISTS (SELECT 1 FROM @CoursesToWaitlist)
        BEGIN
            -- No need to RAISERROR, just return an empty result set for failures.
            -- The application can interpret an empty cart.
            COMMIT; -- Commit the empty transaction
            RETURN;
        END;

        -- Step 3: Perform hard validations (Prerequisites and Time Conflicts)
        DECLARE @currentCourseCode NVARCHAR(50);
        DECLARE @allCartCourses TABLE (CourseCode NVARCHAR(50) PRIMARY KEY);
        INSERT INTO @allCartCourses SELECT CourseCode FROM @CoursesToEnroll UNION SELECT CourseCode FROM @CoursesToWaitlist;

        -- Prerequisite Validation
        DECLARE course_cursor CURSOR FOR SELECT CourseCode FROM @allCartCourses;
        OPEN course_cursor;
        FETCH NEXT FROM course_cursor INTO @currentCourseCode;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF Course.fn_CheckPrerequisiteStatus(@studentId, @currentCourseCode) = 0
            BEGIN
                INSERT INTO @FailedCourses (CourseCode, ErrorMessage) VALUES (@currentCourseCode, 'Prerequisite requirements not met.');
            END
            FETCH NEXT FROM course_cursor INTO @currentCourseCode;
        END;
        CLOSE course_cursor;
        DEALLOCATE course_cursor;

        -- Time Conflict Validation
        -- Part A: Conflicts within the cart
        INSERT INTO @FailedCourses (CourseCode, ErrorMessage)
        SELECT DISTINCT a.CourseCode, 'Time conflict in cart with course: ' + b.CourseCode
        FROM Course.Course a JOIN Course.Course b ON a.CourseCode < b.CourseCode
        WHERE a.CourseCode IN (SELECT CourseCode FROM @allCartCourses)
          AND b.CourseCode IN (SELECT CourseCode FROM @allCartCourses)
          AND a.DayOfWeek = b.DayOfWeek AND a.StartTime < b.EndTime AND b.StartTime < a.EndTime
          AND a.CourseCode NOT IN (SELECT CourseCode FROM @FailedCourses); -- Avoid duplicate error messages

        -- Part B: Conflicts with already enrolled courses
        INSERT INTO @FailedCourses (CourseCode, ErrorMessage)
        SELECT DISTINCT cart.CourseCode, 'Time conflict with enrolled course: ' + enrolled.CourseCode
        FROM Course.Course cart
        JOIN (
            SELECT c_enrolled.* FROM Std.Enrollment e
            JOIN Course.Course c_enrolled ON e.CourseCode = c_enrolled.CourseCode
            WHERE e.StudentId = @studentId AND e.Completed = 0
        ) AS enrolled ON cart.DayOfWeek = enrolled.DayOfWeek AND cart.StartTime < enrolled.EndTime AND enrolled.StartTime < cart.EndTime
        WHERE cart.CourseCode IN (SELECT CourseCode FROM @allCartCourses)
          AND cart.CourseCode NOT IN (SELECT CourseCode FROM @FailedCourses);

        -- Step 4: Remove failed courses from the processing lists
        DELETE FROM @CoursesToEnroll WHERE CourseCode IN (SELECT CourseCode FROM @FailedCourses);
        DELETE FROM @CoursesToWaitlist WHERE CourseCode IN (SELECT CourseCode FROM @FailedCourses);

        -- Step 5: Execute Enrollment and Waitlisting
        -- Enroll in courses that are valid and have seats
        IF EXISTS (SELECT 1 FROM @CoursesToEnroll)
        BEGIN
            INSERT INTO Std.Enrollment (StudentId, GraduationYear, CourseCode, Completed, IsForced, EnrollmentDate)
            SELECT s.StudentId, s.GraduationYear, cte.CourseCode, 0, 0, GETUTCDATE()
            FROM @CoursesToEnroll cte
            JOIN Std.Student s ON s.StudentId = @studentId;
        END;

        -- Waitlist for courses that are valid but full
        IF EXISTS (SELECT 1 FROM @CoursesToWaitlist)
        BEGIN
            INSERT INTO Course.Waitlist (CourseCode, StudentId, GraduationYear, WaitlistTimestamp)
            SELECT ctw.CourseCode, s.StudentId, s.GraduationYear, GETUTCDATE()
            FROM @CoursesToWaitlist ctw
            JOIN Std.Student s ON s.StudentId = @studentId;
        END;

        -- Step 6: Clean up cart (only remove courses that were processed)
        DELETE FROM Std.CourseCart WHERE CourseCode IN (SELECT CourseCode FROM @CoursesToEnroll);
        DELETE FROM Std.CourseCart WHERE CourseCode IN (SELECT CourseCode FROM @CoursesToWaitlist);

        COMMIT TRANSACTION;

        -- Step 7: Report failures back to the application
        SELECT CourseCode, ErrorMessage FROM @FailedCourses;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        -- Re-throw the error to be caught by the application
        THROW;
    END CATCH;
END;
GO
