USE WheresMyScheduleDB;
GO

-- DEGREE CORE COURSES (Department <-> Course)
CREATE TABLE Dept.DegreeCoreCourse (
    DepartmentID VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_dcc PRIMARY KEY (DepartmentID, CourseCode), -- Composite key
    CONSTRAINT fk_dcc_degree
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID),
    CONSTRAINT fk_dcc_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);
GO
