-- Drop the trigger if it already exists to allow for re-creation
DROP TRIGGER IF EXISTS Std.trg_UpdateCourseCounts;
GO

CREATE TRIGGER Std.trg_UpdateCourseCounts
ON Std.Enrollment
AFTER INSERT, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Create a temporary table to hold the course codes and the net change in enrollment
    DECLARE @CourseChanges TABLE (
        CourseCode NVARCHAR(50),
        EnrollmentChange INT
    );

    -- Capture changes from inserts (+1 for each enrollment)
    INSERT INTO @CourseChanges (CourseCode, EnrollmentChange)
    SELECT i.CourseCode, COUNT(*)
    FROM inserted i
    GROUP BY i.CourseCode;

    -- Capture changes from deletes (-1 for each enrollment dropped)
    INSERT INTO @CourseChanges (CourseCode, EnrollmentChange)
    SELECT d.CourseCode, -COUNT(*)
    FROM deleted d
    GROUP BY d.CourseCode;

    -- Aggregate all changes by CourseCode
    DECLARE @AggregatedChanges TABLE (
        CourseCode NVARCHAR(50) PRIMARY KEY,
        TotalChange INT
    );

    INSERT INTO @AggregatedChanges (CourseCode, TotalChange)
    SELECT CourseCode, SUM(EnrollmentChange)
    FROM @CourseChanges
    GROUP BY CourseCode;

    -- Update the Course table in a single statement
    UPDATE c
    SET
        c.EnrolledCount = c.EnrolledCount + ac.TotalChange
    FROM
        Course.Course AS c
    INNER JOIN
        @AggregatedChanges AS ac ON c.CourseCode = ac.CourseCode;

    -- For each course where a student was dropped (TotalChange < 0),
    -- attempt to auto-enroll a student from the waitlist.
    IF EXISTS (SELECT 1 FROM deleted) -- Only proceed if there were deletions
    BEGIN
        DECLARE @DroppedCourseCode VARCHAR(10);

        -- Cursor to iterate through courses that had a student dropped
        DECLARE course_cursor CURSOR FOR
        SELECT CourseCode
        FROM @AggregatedChanges
        WHERE TotalChange < 0; -- Net negative change implies a drop

        OPEN course_cursor;
        FETCH NEXT FROM course_cursor INTO @DroppedCourseCode;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Attempt to auto-enroll from waitlist for this course
            EXEC Course.usp_AutoEnrollFromWaitlist @CourseCode = @DroppedCourseCode;
            FETCH NEXT FROM course_cursor INTO @DroppedCourseCode;
        END;

        CLOSE course_cursor;
        DEALLOCATE course_cursor;
    END;

END;
GO
