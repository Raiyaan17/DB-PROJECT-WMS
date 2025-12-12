-- Insert Test Admin
INSERT INTO [School].[Admin] (AdminID, Username, Email)
VALUES ('ADM001', 'admin', 'admin@school.edu');

-- Get a valid Student ID
SELECT TOP 1 StudentID FROM [Std].[Student];

-- print admin info
SELECT * FROM [School].[Admin] WHERE AdminID = 'ADM001';
