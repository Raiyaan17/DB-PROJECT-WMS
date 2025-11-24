-- SCHOOL
CREATE TABLE School.School (
    SchoolID VARCHAR(30) NOT NULL PRIMARY KEY,
    SchoolName VARCHAR(300) NOT NULL
);
GO

-- SCHOOL-DEPARTMENT MAPPING
CREATE TABLE School.Department (
    SchoolID VARCHAR(30) NOT NULL,
    DepartmentID VARCHAR(30) NOT NULL,
    CONSTRAINT pk_schooldepartment PRIMARY KEY (SchoolID, DepartmentID),
    CONSTRAINT fk_sd_school FOREIGN KEY (SchoolID) REFERENCES School.School(SchoolID),
    CONSTRAINT fk_sd_department FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
GO
