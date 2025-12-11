-- =============================================
-- Description: Retrieves the schedule for a specific student.
-- =============================================
CREATE PROCEDURE Std.sp_GetSchedule
    @studentId VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Check if student exists
    IF NOT EXISTS (SELECT 1 FROM Std.Student WHERE StudentID = @studentId)
    BEGIN
        RAISERROR('Student not found.', 16, 1);
        RETURN;
    END

    -- 2. Query the view for the student's schedule
    SELECT 
        StudentId,
        CourseCode,
        CourseTitle,
        DayOfWeek,
        StartTime,
        EndTime,
        Venue
    FROM Std.vw_StudentSchedule
    WHERE StudentId = @studentId;
END
GO
