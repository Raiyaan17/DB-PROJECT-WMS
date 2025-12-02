USE WheresMyScheduleDB;
GO

CREATE TABLE Dept.Courses (
    CourseCode VARCHAR(10) NOT NULL PRIMARY KEY,
    DepartmentID VARCHAR(30) NOT NULL,

    CONSTRAINT fk_c_dept
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
GO
