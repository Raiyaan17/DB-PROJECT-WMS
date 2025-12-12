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
        SELECT * FROM Course.VwAvailableCourses;
        RETURN;
    END

    SELECT * 
    FROM Course.VwAvailableCourses
    WHERE LOWER(CourseCode) LIKE '%' + LOWER(@query) + '%' 
       OR LOWER(CourseTitle) LIKE '%' + LOWER(@query) + '%';
END;
GO
