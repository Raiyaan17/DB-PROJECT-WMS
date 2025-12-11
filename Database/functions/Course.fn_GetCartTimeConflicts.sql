USE WheresMyScheduleDB;
GO

CREATE OR ALTER FUNCTION Course.fn_GetCartTimeConflicts
(
    @studentId NVARCHAR(450)
)
RETURNS TABLE
AS
RETURN
(
    -- Select courses in the student's cart that have a time conflict with any other course in the same cart
    SELECT
        cc1.CourseCode AS ConflictingCourse1Code,
        c1.CourseTitle AS ConflictingCourse1Title,
        cc2.CourseCode AS ConflictingCourse2Code,
        c2.CourseTitle AS ConflictingCourse2Title,
        c1.DayOfWeek AS ConflictDay,
        c1.StartTime AS ConflictStartTime,
        c1.EndTime AS ConflictEndTime
    FROM
        Std.CourseCart AS cc1
    INNER JOIN
        Course.Course AS c1 ON cc1.CourseCode = c1.CourseCode
    INNER JOIN
        Std.CourseCart AS cc2 ON cc1.StudentId = cc2.StudentId AND cc1.CourseCode < cc2.CourseCode
    INNER JOIN
        Course.Course AS c2 ON cc2.CourseCode = c2.CourseCode
    WHERE
        cc1.StudentId = @studentId
        AND c1.DayOfWeek = c2.DayOfWeek -- Same day of the week
        AND c1.StartTime < c2.EndTime AND c2.StartTime < c1.EndTime -- Check for time overlap
);
