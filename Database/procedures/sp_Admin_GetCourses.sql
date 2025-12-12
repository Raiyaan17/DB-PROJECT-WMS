-- =============================================
-- Description: Get Courses (Optional Dept Filter via Dept.Courses)
-- =============================================
CREATE OR ALTER PROCEDURE sp_Admin_GetCourses
    @DepartmentID VARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @DepartmentID IS NULL
    BEGIN
        SELECT 
            c.CourseCode,
            c.CourseTitle,
            c.TotalCredits,
            c.Capacity,
            c.Venue,
            c.DayOfWeek,
            c.StartTime,
            c.EndTime,
            c.EnrolledCount,
            c.RemainingSeats,
            c.IsActive
        FROM Course.Course c
        ORDER BY c.CourseCode;
    END
    ELSE
    BEGIN
        SELECT 
            c.CourseCode,
            c.CourseTitle,
            c.TotalCredits,
            c.Capacity,
            c.Venue,
            c.DayOfWeek,
            c.StartTime,
            c.EndTime,
            c.EnrolledCount,
            c.RemainingSeats,
            c.IsActive
        FROM Course.Course c
        JOIN Dept.Courses dc ON c.CourseCode = dc.CourseCode
        WHERE dc.DepartmentID = @DepartmentID
        ORDER BY c.CourseCode;
    END
END;
GO
