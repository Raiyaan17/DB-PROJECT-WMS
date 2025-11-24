USE WheresMyScheduleDB;
GO

CREATE OR ALTER FUNCTION fn_GetRemainingSeats (@CourseCode VARCHAR(10))
RETURNS INT
AS
BEGIN
    DECLARE @Capacity SMALLINT;
    DECLARE @EnrolledCount INT;
    DECLARE @Remaining INT;

    --get the max capacity and tracked seats from course table
    SELECT 
        @Capacity = Capacity,
        @Remaining = RemainingSeats
    FROM Course.Course 
    WHERE CourseCode = @CourseCode;

    IF @Capacity IS NULL
        RETURN 0;

    --count how many students are currently enrolled (if completed =0 they are currently taking it)
    IF @Remaining IS NULL
    BEGIN
        SELECT @EnrolledCount = COUNT(*)
        FROM Std.Enrollment
        WHERE CourseCode = @CourseCode 
        AND Completed = 0;

        --calculate remaining seats
        SET @Remaining = @Capacity - ISNULL(@EnrolledCount, 0);
    END

    RETURN @Remaining;
END;
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
