USE WheresMyScheduleDB;
GO

CREATE OR ALTER FUNCTION Course.fn_GetRemainingSeats (@CourseCode VARCHAR(10))
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
