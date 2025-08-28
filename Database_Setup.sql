-- Create Tables
CREATE TABLE Departments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Inquiries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    DepartmentId INT NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
);

-- Insert Sample Data
INSERT INTO Departments (Name) VALUES 
('משאבי אנוש'), 
('טכנולוגיות מידע'), 
('שירות לקוחות'), 
('כספים');

-- Stored Procedure for Monthly Report
GO
CREATE PROCEDURE GetMonthlyInquiriesReport
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Current month data
    WITH CurrentMonth AS (
        SELECT 
            d.Name AS DepartmentName,
            COUNT(i.Id) AS CurrentMonthCount
        FROM Departments d
        LEFT JOIN Inquiries i ON d.Id = i.DepartmentId 
            AND YEAR(i.CreatedAt) = @Year 
            AND MONTH(i.CreatedAt) = @Month
        GROUP BY d.Id, d.Name
    ),
    -- Previous month data
    PreviousMonth AS (
        SELECT 
            d.Name AS DepartmentName,
            COUNT(i.Id) AS PreviousMonthCount
        FROM Departments d
        LEFT JOIN Inquiries i ON d.Id = i.DepartmentId 
            AND ((YEAR(i.CreatedAt) = @Year AND MONTH(i.CreatedAt) = @Month - 1)
                OR (YEAR(i.CreatedAt) = @Year - 1 AND MONTH(i.CreatedAt) = 12 AND @Month = 1))
        GROUP BY d.Id, d.Name
    ),
    -- Same month last year
    LastYear AS (
        SELECT 
            d.Name AS DepartmentName,
            COUNT(i.Id) AS LastYearCount
        FROM Departments d
        LEFT JOIN Inquiries i ON d.Id = i.DepartmentId 
            AND YEAR(i.CreatedAt) = @Year - 1 
            AND MONTH(i.CreatedAt) = @Month
        GROUP BY d.Id, d.Name
    )
    
    SELECT 
        cm.DepartmentName,
        cm.CurrentMonthCount,
        pm.PreviousMonthCount,
        ly.LastYearCount,
        -- Calculate differences
        (cm.CurrentMonthCount - pm.PreviousMonthCount) AS DiffFromPreviousMonth,
        (cm.CurrentMonthCount - ly.LastYearCount) AS DiffFromLastYear,
        -- Calculate percentages
        CASE 
            WHEN pm.PreviousMonthCount = 0 THEN NULL
            ELSE CAST((cm.CurrentMonthCount - pm.PreviousMonthCount) * 100.0 / pm.PreviousMonthCount AS DECIMAL(5,2))
        END AS PercentChangeFromPreviousMonth,
        CASE 
            WHEN ly.LastYearCount = 0 THEN NULL
            ELSE CAST((cm.CurrentMonthCount - ly.LastYearCount) * 100.0 / ly.LastYearCount AS DECIMAL(5,2))
        END AS PercentChangeFromLastYear
    FROM CurrentMonth cm
    JOIN PreviousMonth pm ON cm.DepartmentName = pm.DepartmentName
    JOIN LastYear ly ON cm.DepartmentName = ly.DepartmentName
    ORDER BY cm.CurrentMonthCount DESC;
END