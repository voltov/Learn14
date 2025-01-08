CREATE TABLE Product (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Weight DECIMAL(10, 2),
    Height DECIMAL(10, 2),
    Width DECIMAL(10, 2),
    Length DECIMAL(10, 2)
);

CREATE TABLE [Order] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Status NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ProductId INT,
    FOREIGN KEY (ProductId) REFERENCES Product(Id)
);
