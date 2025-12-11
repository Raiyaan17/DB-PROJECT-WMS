-- write a select query to output 10 latest students by graduation year with all their info
SELECT TOP 10 * FROM Std.Student ORDER BY GraduationYear DESC, StudentId DESC;

-- show the in cart courses
SELECT * FROM Std.CourseCart;

-- return courses
SELECT * FROM Course.Course;

-- print waitlist
SELECT * FROM Course.Waitlist;

-- print enrolled
SELECT * FROM Std.Enrollment;

-- departments
SELECT * FROM Dept.Department;

-- professors
SELECT * FROM Inst.Instructor;

-- how many instructors in each department, count using department id 
SELECT COUNT(*) AS TotalInstructors FROM Inst.Instructor;

SELECT
    d.DepartmentName,
    COUNT(i.InstructorID) AS NumberOfInstructors
FROM
    Inst.Instructor i
JOIN
    Dept.Department d ON i.DepartmentID = d.DepartmentID
GROUP BY
    d.DepartmentName
ORDER BY
    NumberOfInstructors DESC;  





