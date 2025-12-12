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
    d.DepartmentID,
    d.DepartmentName,
    STRING_AGG(i.FName + ' ' + i.LName, ', ') AS InstructorName
FROM Course.Course c
JOIN Dept.Courses dc ON c.CourseCode = dc.CourseCode
JOIN Dept.Department d ON dc.DepartmentID = d.DepartmentID
LEFT JOIN Inst.TeachingAssignment ta ON c.CourseCode = ta.CourseCode
LEFT JOIN Inst.Instructor i ON ta.InstructorID = i.InstructorID
WHERE c.IsActive = 1
GROUP BY 
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
    d.DepartmentID,
    d.DepartmentName;
GO
