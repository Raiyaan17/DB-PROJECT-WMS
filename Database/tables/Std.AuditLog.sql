USE WheresMyScheduleDB;
GO

-- Adding Audit table needed for force enrollment 
CREATE TABLE Std.AuditLog (
    AuditID INT IDENTITY(1,1) PRIMARY KEY,
    AdminID VARCHAR(30) NULL,
    StudentID VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    ActionDescription VARCHAR(200) NOT NULL
);
GO
