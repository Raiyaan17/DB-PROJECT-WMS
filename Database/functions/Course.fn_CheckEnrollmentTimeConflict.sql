USE WheresMyScheduleDB;
GO

CREATE OR ALTER FUNCTION Course.fn_CheckEnrollmentTimeConflict
(
    @studentId VARCHAR(30),
    @proposedCourseCode VARCHAR(10)
)
RETURNS BIT
AS
BEGIN
    DECLARE @ConflictCount INT;

    SELECT @ConflictCount = COUNT(e.CourseCode)
    FROM Std.Enrollment AS e
    INNER JOIN Course.Course AS ec ON e.CourseCode = ec.CourseCode -- Enrolled Course details
    INNER JOIN Course.Course AS pc ON pc.CourseCode = @proposedCourseCode -- Proposed Course details
    WHERE
        e.StudentID = @studentId
        AND e.Completed = 0 -- Only consider actively enrolled courses
        AND ec.DayOfWeek = pc.DayOfWeek -- Same day of the week
        AND ec.StartTime < pc.EndTime
        AND pc.StartTime < ec.EndTime;

    IF @ConflictCount > 0
        RETURN 1; -- Conflict found
    ELSE
        RETURN 0; -- No conflict
END;
