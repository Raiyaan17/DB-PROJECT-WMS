

USE WheresMyScheduleDB;
GO


IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Enrollment_StudentID'
      AND object_id = OBJECT_ID('Std.Enrollment')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Enrollment_StudentID
        ON Std.Enrollment (StudentID);
END;
GO



IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Course_Capacity_Open'
      AND object_id = OBJECT_ID('Course.Course')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Course_Capacity_Open
        ON Course.Course (Capacity)
        WHERE Capacity > 0;
END;
GO
