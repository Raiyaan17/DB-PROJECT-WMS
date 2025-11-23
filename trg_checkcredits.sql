CREATE TRIGGER trg_CheckCreditLimit
ON Std.Enrollment
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StudentID VARCHAR(30);
    DECLARE @CurrentCredits INT;
    DECLARE @CreditLimit INT = 21; -- A hardcoded limit for all students (cannot exceed 21 credits in a given semester)

    SELECT @StudentID = StudentID FROM inserted;

    --calculate the total credits for currently active courses (completed =0)
    SELECT @CurrentCredits = SUM(C.TotalCredits)
    FROM Std.Enrollment E
    JOIN Course.Course C ON E.CourseCode = C.CourseCode
    WHERE E.StudentID = @StudentID
    AND E.Completed = 0; -- only count ongoing courses

    --actual validation step
    IF @CurrentCredits > @CreditLimit
    BEGIN
        RAISERROR ('Enrollment failed... cannot exceed the credit limit (21).', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO
