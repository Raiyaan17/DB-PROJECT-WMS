USE WheresMyScheduleDB;
GO

-- 4. Search Courses
CREATE OR ALTER PROCEDURE Course.sp_SearchCourses
    @query NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @query IS NULL OR @query = ''
    BEGIN
        SELECT * FROM Course.vw_AvailableCourses;
        RETURN;
    END

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
    WHERE LOWER(CourseCode) LIKE '%' + LOWER(@query) + '%' 
       OR LOWER(CourseTitle) LIKE '%' + LOWER(@query) + '%';
END;
GO
