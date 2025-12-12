USE WheresMyScheduleDB;
GO

-- 1. Get Cart
CREATE OR ALTER PROCEDURE Std.sp_GetCart
    @studentId VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        cc.StudentId,
        cc.GraduationYear,
        cc.CourseCode,
        c.CourseTitle,
        c.TotalCredits
    FROM Std.CourseCart cc
    JOIN Course.Course c ON cc.CourseCode = c.CourseCode
    WHERE cc.StudentId = @studentId;
END;
GO
