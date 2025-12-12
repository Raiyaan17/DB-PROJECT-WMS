USE WheresMyScheduleDB;
GO

CREATE OR ALTER PROCEDURE Course.sp_UpdateCapacity
    @CourseCode VARCHAR(10),
    @Capacity   SMALLINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EnrolledCount INT;

    SELECT @EnrolledCount = EnrolledCount
    FROM Course.Course
    WHERE CourseCode = @CourseCode;

    IF @EnrolledCount IS NULL
        RAISERROR('Course does not exist.', 16, 1);

    IF @Capacity < @EnrolledCount
        RAISERROR('New capacity cannot be less than current enrollment.', 16, 1);

    UPDATE Course.Course
    SET Capacity = @Capacity
    WHERE CourseCode = @CourseCode;
END
GO
