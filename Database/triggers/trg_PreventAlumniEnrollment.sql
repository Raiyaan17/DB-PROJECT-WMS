USE WheresMyScheduleDB;
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
