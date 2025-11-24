USE WheresMyScheduleDB;
GO

CREATE OR ALTER TRIGGER trg_CheckCreditLimit
ON Std.Enrollment
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CreditLimit INT = 21; -- A hardcoded limit for all students (cannot exceed 21 credits in a given semester)

    ;WITH StudentsToCheck AS (
        SELECT DISTINCT StudentID
        FROM inserted
        WHERE ISNULL(IsForced, 0) = 0
    ),
    Credits AS (
        --calculate the total credits for currently active courses (completed =0)
        SELECT s.StudentID,
               SUM(C.TotalCredits) AS CurrentCredits
        FROM StudentsToCheck s
        JOIN Std.Enrollment E ON E.StudentID = s.StudentID
        JOIN Course.Course C ON E.CourseCode = C.CourseCode
        WHERE E.Completed = 0 -- only count ongoing courses
        GROUP BY s.StudentID
    )
    --actual validation step
    IF EXISTS (
        SELECT 1
        FROM Credits
        WHERE CurrentCredits > @CreditLimit
    )
    BEGIN
        RAISERROR ('Enrollment failed... cannot exceed the credit limit (21).', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

CREATE OR ALTER TRIGGER trg_PreventAlumniEnrollment
ON Std.Enrollment
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    --check if the person enrolling is an alumni
    IF EXISTS (
        SELECT 1 
        FROM Std.Student S
        JOIN inserted i ON S.StudentID = i.StudentID
        WHERE S.CurrentAcademicYear = 'ALUMNI'
        AND ISNULL(i.IsForced, 0) = 0
    )
    BEGIN
        RAISERROR ('Enrollment failed... alumni are not allowed to enroll.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO
