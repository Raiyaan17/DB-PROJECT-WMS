USE WheresMyScheduleDB;
GO

-- 3. Get Departments
CREATE OR ALTER PROCEDURE Course.sp_GetDepartments
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        DepartmentID,
        DepartmentName
    FROM Dept.Department;
END;
GO
