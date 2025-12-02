USE WheresMyScheduleDB;
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
