-- Encapsulate the creation of the partitioned student table into a stored procedure for clarity.
GO
CREATE PROCEDURE dbo.SetupStudentTablePartitioning
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the partition function already exists
    IF NOT EXISTS (SELECT 1 FROM sys.partition_functions WHERE name = 'pf_GraduatingYear')
    BEGIN
        PRINT 'Creating partition function pf_GraduatingYear...';
        CREATE PARTITION FUNCTION pf_GraduatingYear (SMALLINT)
        AS RANGE RIGHT FOR VALUES (
            1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004,
            2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014,
            2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024,
            2025, 2026, 2027, 2028, 2029, 2030
        );
    END

    -- Check if the partition scheme already exists
    IF NOT EXISTS (SELECT 1 FROM sys.partition_schemes WHERE name = 'ps_GraduatingYear')
    BEGIN
        PRINT 'Creating partition scheme ps_GraduatingYear...';
        CREATE PARTITION SCHEME ps_GraduatingYear
        AS PARTITION pf_GraduatingYear
        ALL TO ([PRIMARY]);
    END

    -- Check if the table already exists
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Student' AND schema_id = SCHEMA_ID('Std'))
    BEGIN
        PRINT 'Creating partitioned table Std.Student...';
        CREATE TABLE Std.Student (
            StudentID             VARCHAR(30)  NOT NULL,
            FName                 VARCHAR(50)  NOT NULL,
            LName                 VARCHAR(50)  NOT NULL,
            Email                 VARCHAR(100) NOT NULL,
            SchoolID              VARCHAR(30)  NOT NULL,
            DepartmentID          VARCHAR(30)  NOT NULL,
            GraduationYear        SMALLINT  NOT NULL,
            CurrentAcademicYear   VARCHAR(10)  NULL,

            CONSTRAINT uq_student_email UNIQUE (GraduationYear, Email),
            CONSTRAINT pk_student PRIMARY KEY CLUSTERED (GraduationYear, StudentID),
            CONSTRAINT fk_student_school
                FOREIGN KEY (SchoolID) REFERENCES School.School(SchoolID),
            CONSTRAINT fk_student_department
                FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID),
            CONSTRAINT chk_student_acad_year
                CHECK (
                    CurrentAcademicYear IN ('FRESHMAN','SOPHOMORE','JUNIOR','SENIOR', 'ALUMNI')
                    OR CurrentAcademicYear IS NULL
                )
        ) ON ps_GraduatingYear(GraduationYear);
    END
END
GO



