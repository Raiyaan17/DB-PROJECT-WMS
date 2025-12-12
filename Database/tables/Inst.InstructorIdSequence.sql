USE WheresMyScheduleDB;
GO

-- InstructorID helper table
CREATE TABLE Inst.InstructorIdSequence (
    DepartmentID VARCHAR(30) NOT NULL PRIMARY KEY,
    LastNumber INT         NOT NULL,
    CONSTRAINT fk_iis_department
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
GO
