

USE WheresMyScheduleDB;
GO


DECLARE @TargetCourseCode VARCHAR(10) = 'CS100';   

;WITH cte_PrerequisitePath AS (
    -- Anchor: direct prerequisites of the target course
    SELECT
        cp.CourseCode,
        cp.PrerequisiteCode,
        1 AS Depth
    FROM Course.CoursePrerequisite AS cp
    WHERE cp.CourseCode = @TargetCourseCode

    UNION ALL

    -- Recursive: prerequisites of each prerequisite
    SELECT
        cp2.CourseCode,
        cp2.PrerequisiteCode,
        cte.Depth + 1 AS Depth
    FROM Course.CoursePrerequisite AS cp2
    JOIN cte_PrerequisitePath AS cte
        ON cp2.CourseCode = cte.PrerequisiteCode
)
SELECT
    CourseCode,
    PrerequisiteCode,
    Depth
FROM cte_PrerequisitePath
ORDER BY Depth, CourseCode, PrerequisiteCode;
GO


DECLARE @StudentID        VARCHAR(30) = '27100305';  
DECLARE @TargetCourse2    VARCHAR(10) = 'CS200';      

;WITH cte_StudentScheduleClashes AS (
    SELECT
        e.StudentID,
        e.CourseCode,
        e.Completed      
    FROM Std.Enrollment AS e
    WHERE e.StudentID = @StudentID
)
SELECT
    StudentID,
    CourseCode,
    Completed
FROM cte_StudentScheduleClashes
WHERE CourseCode = @TargetCourse2;
GO
