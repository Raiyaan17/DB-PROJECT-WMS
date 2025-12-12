USE WheresMyScheduleDB;
GO

-- DEGREE ELECTIVE COURSES (Department <-> Course)
CREATE TABLE Dept.DegreeElectiveCourse (
    DepartmentID VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_dec PRIMARY KEY (DepartmentID, CourseCode), -- Composite key
    CONSTRAINT fk_dec_degree
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID),
    CONSTRAINT fk_dec_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);
GO
