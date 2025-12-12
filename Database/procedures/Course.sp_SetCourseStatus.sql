USE WheresMyScheduleDB;
GO

CREATE OR ALTER PROCEDURE Course.sp_SetCourseStatus
    @CourseCode VARCHAR(10),
    @IsActive   BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Course.Course WHERE CourseCode = @CourseCode)
        RAISERROR('Course does not exist.', 16, 1);

    UPDATE Course.Course
    SET IsActive = @IsActive
    WHERE CourseCode = @CourseCode;
END
GO
