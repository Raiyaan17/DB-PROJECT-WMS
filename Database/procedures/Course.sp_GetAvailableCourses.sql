-- =============================================
-- Description: Retrieves available courses, optionally filtered by department.
-- =============================================
CREATE OR ALTER PROCEDURE Course.sp_GetAvailableCourses
    @departmentCode VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CourseCode,
        CourseTitle,
        TotalCredits,
        DepartmentID,
        DepartmentName,
        EnrolledCount,
        Capacity,
        RemainingSeats,
        DayOfWeek,
        StartTime,
        EndTime,
        Venue,
        InstructorName
    FROM Course.vw_AvailableCourses
    WHERE 
        (@departmentCode IS NULL OR DepartmentID = @departmentCode);
END
GO
