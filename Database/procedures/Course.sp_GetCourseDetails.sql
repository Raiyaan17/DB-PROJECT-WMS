-- =============================================
-- Description: Retrieves detailed information for a course, including its prerequisites.
-- =============================================
CREATE PROCEDURE Course.sp_GetCourseDetails
    @courseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the course exists
    IF NOT EXISTS (SELECT 1 FROM Course.Course WHERE CourseCode = @courseCode)
    BEGIN
        -- Return an empty result set if the course doesn't exist, C# layer will handle null
        RETURN;
    END

    -- Using a LEFT JOIN to ensure the course details are returned even if there are no prerequisites
    SELECT 
        c.CourseCode,
        c.CourseTitle,
        c.TotalCredits,
        c.Capacity,
        c.EnrolledCount,
        c.RemainingSeats,
        c.Venue,
        c.DayOfWeek,
        c.StartTime,
        c.EndTime,
        c.IsActive,
        prereq.PrerequisiteCode,
        prereq.PrerequisiteTitle
    FROM 
        Course.Course c
    LEFT JOIN 
        Course.vw_FullPrerequisiteList prereq ON c.CourseCode = prereq.CourseCode
    WHERE 
        c.CourseCode = @courseCode;
END
GO
