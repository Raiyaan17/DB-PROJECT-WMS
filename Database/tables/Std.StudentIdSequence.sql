USE WheresMyScheduleDB;
GO

-- StudentID helper table
-- StudentID = year_joined + DepartmentID + last_number;
-- e.g. '2023CS1', '2023CS2', '2024MGS1', '2021ACF13'
CREATE TABLE Std.StudentIdSequence (
    YearJoined SMALLINT    NOT NULL,
    DepartmentID VARCHAR(30) NOT NULL,
    LastNumber INT         NOT NULL,
    CONSTRAINT pk_StudentIdSequence
        PRIMARY KEY (YearJoined, DepartmentID),
    CONSTRAINT fk_sis_department
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
GO
