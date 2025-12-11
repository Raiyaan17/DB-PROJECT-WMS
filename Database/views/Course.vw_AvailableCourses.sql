USE WheresMyScheduleDB;
GO

-----------------------------------------------------------
-- VIEW 1: vw_AvailableCourses
-- Used by students to browse open & active courses.
-----------------------------------------------------------
CREATE OR ALTER VIEW Course.vw_AvailableCourses AS
SELECT 
    c.CourseCode,
    c.CourseTitle,
    c.TotalCredits,
    c.Venue,
    c.DayOfWeek,
    c.StartTime,
    c.EndTime,
    c.EnrolledCount,
    c.Capacity,
    c.RemainingSeats,
    d.DepartmentName
FROM Course.Course c
JOIN Dept.Courses dc ON c.CourseCode = dc.CourseCode
JOIN Dept.Department d ON dc.DepartmentID = d.DepartmentID
WHERE c.IsActive = 1;
GO
