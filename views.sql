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
    c.RemainingSeats,
    d.DepartmentName
FROM Course.Course c
JOIN Dept.Courses dc ON c.CourseCode = dc.CourseCode
JOIN Dept.Department d ON dc.DepartmentID = d.DepartmentID
WHERE c.IsActive = 1;
GO

-----------------------------------------------------------
-- VIEW 2: vw_StudentSchedule
-- Weekly schedule view for all students.
-- Used to render student calendar UI.
-----------------------------------------------------------
CREATE OR ALTER VIEW Std.vw_StudentSchedule AS
SELECT 
    e.StudentID,
    e.CourseCode,
    c.CourseTitle,
    c.DayOfWeek,
    c.StartTime,
    c.EndTime,
    c.Venue
FROM Std.Enrollment e
JOIN Course.Course c ON e.CourseCode = c.CourseCode;
GO

-----------------------------------------------------------
-- VIEW 3: vw_AdminDashboardStats
-- High-level admin metrics:
-- total students, instructors, courses by department.
-----------------------------------------------------------
CREATE OR ALTER VIEW School.vw_AdminDashboardStats AS
SELECT 
    d.DepartmentName,
    COUNT(DISTINCT s.StudentID) AS TotalStudents,
    COUNT(DISTINCT i.InstructorID) AS TotalInstructors,
    COUNT(DISTINCT dc.CourseCode) AS TotalCourses
FROM Dept.Department d
LEFT JOIN Std.Student s ON s.DepartmentID = d.DepartmentID
LEFT JOIN Inst.Instructor i ON i.DepartmentID = d.DepartmentID
LEFT JOIN Dept.Courses dc ON dc.DepartmentID = d.DepartmentID
GROUP BY d.DepartmentName;
GO
-----------------------------------------------------------
-- VIEW 4: vw_FullPrerequisiteList
-- Shows every course and its required prerequisites.
-- for prerequisite validation.
-----------------------------------------------------------
CREATE OR ALTER VIEW Course.vw_FullPrerequisiteList AS
SELECT 
    cp.CourseCode,
    c.CourseTitle,
    cp.PrerequisiteCode,
    p.CourseTitle AS PrerequisiteTitle
FROM Course.CoursePrerequisite cp
JOIN Course.Course c ON cp.CourseCode = c.CourseCode
JOIN Course.Course p ON cp.PrerequisiteCode = p.CourseCode;
GO
-----------------------------------------------------------
-- VIEW 5: vw_StudentTranscript
-- Shows a student's completed & ongoing courses.
-----------------------------------------------------------
CREATE OR ALTER VIEW Std.vw_StudentTranscript AS
SELECT 
    e.StudentID,
    e.CourseCode,
    c.CourseTitle,
    c.TotalCredits,
    e.Completed,
    e.EnrollmentDate
FROM Std.Enrollment e
JOIN Course.Course c ON e.CourseCode = c.CourseCode;
GO
