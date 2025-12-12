-- =============================================
-- Description: Auto-generates InstructorID on Insert
-- =============================================
CREATE OR ALTER TRIGGER Inst.trg_GenerateInstructorID
ON Inst.Instructor
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FName VARCHAR(50);
    DECLARE @LName VARCHAR(50);
    DECLARE @Email VARCHAR(100);
    DECLARE @DepartmentID VARCHAR(30);
    DECLARE @NewID VARCHAR(30);
    DECLARE @NextNum INT;

    -- Cursor to handle bulk inserts (good practice)
    DECLARE cur_Insert CURSOR FOR
        SELECT FName, LName, Email, DepartmentID
        FROM inserted;

    OPEN cur_Insert;
    FETCH NEXT FROM cur_Insert INTO @FName, @LName, @Email, @DepartmentID;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- 1. Get Next Number for Department
        IF NOT EXISTS (SELECT 1 FROM Inst.InstructorIdSequence WHERE DepartmentID = @DepartmentID)
        BEGIN
            -- Initialize sequence for this department if not exists
            INSERT INTO Inst.InstructorIdSequence (DepartmentID, LastNumber)
            VALUES (@DepartmentID, 0);
        END

        UPDATE Inst.InstructorIdSequence
        SET LastNumber = LastNumber + 1
        WHERE DepartmentID = @DepartmentID;

        SELECT @NextNum = LastNumber
        FROM Inst.InstructorIdSequence
        WHERE DepartmentID = @DepartmentID;

        -- 2. Format ID
        SET @NewID = @DepartmentID + CAST(@NextNum AS VARCHAR);

        -- 3. Insert into Table
        INSERT INTO Inst.Instructor (InstructorID, FName, LName, Email, DepartmentID)
        VALUES (@NewID, @FName, @LName, @Email, @DepartmentID);

        FETCH NEXT FROM cur_Insert INTO @FName, @LName, @Email, @DepartmentID;
    END

    CLOSE cur_Insert;
    DEALLOCATE cur_Insert;
END;
GO
