USE WheresMyScheduleDB;
GO

CREATE OR ALTER FUNCTION Course.fn_CheckPrerequisiteStatus 
(
    @StudentID VARCHAR(30), 
    @TargetCourseCode VARCHAR(10)
)
RETURNS BIT
AS
BEGIN
    DECLARE @UnmetPrereqs INT;

    --invalid course code should not mark prerequisites as satisfied
    IF NOT EXISTS (
        SELECT 1
        FROM Course.Course
        WHERE CourseCode = @TargetCourseCode
    )
        RETURN 0;

    --Count how many prereqs exist for the given course 
    --specifically that the student HAS NOT completed
    SELECT @UnmetPrereqs = COUNT(*)
    FROM Course.CoursePrerequisite CP
    WHERE CP.CourseCode = @TargetCourseCode --the course that they want to take
    AND NOT EXISTS (
        --list of courses that the student has taken alr
        SELECT 1
        FROM Std.Enrollment
        WHERE StudentID = @StudentID 
        AND Completed = 1
        AND CourseCode = CP.PrerequisiteCode
    );

    --unment must be 0 otherwise return false
    IF @UnmetPrereqs = 0 
        RETURN 1;

    RETURN 0;
END;
GO
