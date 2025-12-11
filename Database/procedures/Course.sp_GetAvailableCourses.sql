-- =============================================
-- Description: Retrieves available courses, optionally filtered by department.
-- =============================================
CREATE PROCEDURE Course.sp_GetAvailableCourses
    @departmentCode VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CourseCode,
        CourseTitle,
        TotalCredits,
        DepartmentName,
        EnrolledCount,
        Capacity,
        RemainingSeats,
        DayOfWeek,
        StartTime,
        EndTime,
        Venue
    FROM Course.vw_AvailableCourses
    WHERE 
        (@departmentCode IS NULL OR DepartmentName = @departmentCode);
END
GO
