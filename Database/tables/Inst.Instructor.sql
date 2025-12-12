USE WheresMyScheduleDB;
GO

-- INSTRUCTORS
CREATE TABLE Inst.Instructor (
    InstructorID VARCHAR(30) NOT NULL PRIMARY KEY, -- eg. CS1, MGS10
    FName        VARCHAR(50) NOT NULL,
    LName        VARCHAR(50) NOT NULL,
    Email        VARCHAR(100) NOT NULL UNIQUE,     -- eg. fname.lname.InstructorID@lums.edu.pk
    DepartmentID   VARCHAR(30) NOT NULL,
    CONSTRAINT fk_instructor_dept
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
GO
