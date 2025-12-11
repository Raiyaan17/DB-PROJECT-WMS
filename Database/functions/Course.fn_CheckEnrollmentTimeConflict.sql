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
    INNER JOIN Course.Course AS ec ON e.CourseCode = ec.CourseCode 
    INNER JOIN Course.Course AS pc ON pc.CourseCode = @proposedCourseCode 
    WHERE
        e.StudentID = @studentId
        AND e.Completed = 0 
        AND ec.DayOfWeek = pc.DayOfWeek 
        AND ec.StartTime < pc.EndTime
        AND pc.StartTime < ec.EndTime;

    -- If conflict exists, return 1 and exit immediately
    IF @ConflictCount > 0
        RETURN 1; 

    -- If we reached this line, there is no conflict. 
    -- This satisfies the "last statement must be return" rule.
    RETURN 0; 
END;
GO