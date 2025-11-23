CREATE FUNCTION Course.fn_CheckPrerequisiteStatus 
(
    @StudentID VARCHAR(30), 
    @TargetCourseCode VARCHAR(10)
)
RETURNS BIT
AS
BEGIN
    DECLARE @UnmetPrereqs INT;

    --Count how many prereqs exist for the given course 
    --specifically that the student HAS NOT completed
    SELECT @UnmetPrereqs = COUNT(*)
    FROM Course.CoursePrerequisite CP
    WHERE CP.CourseCode = @TargetCourseCode --the course that they want to take
    AND CP.PrerequisiteCode NOT IN (
        --list of courses that the student has taken alr
        SELECT CourseCode 
        FROM Std.Enrollment
        WHERE StudentID = @StudentID 
        AND Completed = 1
    );

    --unment must be 0 otherwise return false
    IF @UnmetPrereqs = 0 
        RETURN 1;

    RETURN 0;
END;
GO
