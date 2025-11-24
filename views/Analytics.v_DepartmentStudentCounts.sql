-- =============================================
-- Description:   View to count the number of students per department using a CTE.
-- Usage:         SELECT * FROM Analytics.v_DepartmentStudentCounts;
-- =============================================
IF OBJECT_ID('Analytics.v_DepartmentStudentCounts', 'V') IS NOT NULL
    DROP VIEW Analytics.v_DepartmentStudentCounts;
GO

CREATE VIEW Analytics.v_DepartmentStudentCounts
AS
    WITH DepartmentStudentData AS (
        SELECT
            s.StudentID,
            s.DepartmentID,
            d.DepartmentName
        FROM Std.Student s
        INNER JOIN Dept.Department d ON s.DepartmentID = d.DepartmentID
    )
    SELECT
        ds.DepartmentID,
        ds.DepartmentName,
        COUNT(ds.StudentID) AS NumberOfStudents
    FROM DepartmentStudentData ds
    GROUP BY
        ds.DepartmentID,
        ds.DepartmentName;
GO
