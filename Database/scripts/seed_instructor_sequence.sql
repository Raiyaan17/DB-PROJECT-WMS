-- =============================================
-- Description: Seeds the InstructorIdSequence table based on existing data
-- =============================================

PRINT 'Seeding Inst.InstructorIdSequence from existing data...';

MERGE INTO Inst.InstructorIdSequence AS Target
USING (
    SELECT
        DepartmentID,
        MAX(CAST(REPLACE(InstructorID, DepartmentID, '') AS INT)) as MaxNum
    FROM Inst.Instructor
    WHERE InstructorID LIKE DepartmentID + '%' -- Ensure we only look at valid IDs
      AND ISNUMERIC(REPLACE(InstructorID, DepartmentID, '')) = 1
    GROUP BY DepartmentID
) AS Source
ON (Target.DepartmentID = Source.DepartmentID)
WHEN MATCHED THEN
    UPDATE SET Target.LastNumber = Source.MaxNum
WHEN NOT MATCHED THEN
    INSERT (DepartmentID, LastNumber)
    VALUES (Source.DepartmentID, Source.MaxNum);

PRINT 'Seeding complete.';
GO
