USE WheresMyScheduleDB;
GO

CREATE OR ALTER TRIGGER trg_CheckCreditLimit
ON Std.Enrollment
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CreditLimit INT = 21;
    DECLARE @ViolatingStudentCount INT = 0;

    -- A CTE's scope is only the single statement that immediately follows it.
    -- Here, we define the CTE to calculate current credits for each student in the transaction...
    -- A CTE's scope is only the single statement that immediately follows it.
    -- Here, we define the CTE to calculate current credits for each student in the transaction...
    ;WITH InvolvedStudents AS (
        SELECT DISTINCT StudentID FROM inserted
    ),
    StudentCredits AS (
        SELECT
            s.StudentID,
            SUM(c.TotalCredits) AS CurrentCredits
        FROM InvolvedStudents s
        JOIN Std.Enrollment e ON s.StudentID = e.StudentID
        JOIN Course.Course c ON e.CourseCode = c.CourseCode
        WHERE e.Completed = 0 AND ISNULL(e.IsForced, 0) = 0
        GROUP BY s.StudentID
    )
    -- ...and this SELECT statement is the required statement that immediately follows and uses the CTE.
    -- We use it to count how many students from the CTE are over the credit limit.
    SELECT @ViolatingStudentCount = COUNT(*)
    FROM StudentCredits
    WHERE CurrentCredits > @CreditLimit;

    -- Finally, we check the count in a separate statement.
    IF @ViolatingStudentCount > 0
    BEGIN
        RAISERROR ('Enrollment failed... cannot exceed the credit limit (21).', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO
