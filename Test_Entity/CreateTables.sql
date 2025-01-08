CREATE PROCEDURE GetFilteredOrders
    @Year INT = NULL,
    @Month INT = NULL,
    @Status NVARCHAR(50) = NULL,
    @ProductId INT = NULL
AS
BEGIN
    SELECT * FROM [Order]
    WHERE (@Year IS NULL OR YEAR(CreatedDate) = @Year)
      AND (@Month IS NULL OR MONTH(CreatedDate) = @Month)
      AND (@Status IS NULL OR Status = @Status)
      AND (@ProductId IS NULL OR ProductId = @ProductId)
END
