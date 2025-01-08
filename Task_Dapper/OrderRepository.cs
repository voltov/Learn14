using Dapper;

namespace Task_Dapper
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM [Order] o JOIN Product p ON o.ProductId = p.Id";
                var orderDictionary = new Dictionary<int, Order>();

                var orders = await connection.QueryAsync<Order, Product, Order>(sql, (order, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.Product = product;
                        orderDictionary.Add(orderEntry.Id, orderEntry);
                    }
                    return orderEntry;
                });

                return orders.Distinct().ToList();
            }
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM [Order] o JOIN Product p ON o.ProductId = p.Id WHERE o.Id = @Id";
                var orderDictionary = new Dictionary<int, Order>();

                var orders = await connection.QueryAsync<Order, Product, Order>(sql, (order, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.Product = product;
                        orderDictionary.Add(orderEntry.Id, orderEntry);
                    }
                    return orderEntry;
                }, new { Id = id });

                return orders.FirstOrDefault();
            }
        }

        public async Task AddOrderAsync(Order order)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "INSERT INTO [Order] (Status, CreatedDate, UpdatedDate, ProductId) VALUES (@Status, @CreatedDate, @UpdatedDate, @ProductId)";
                await connection.ExecuteAsync(sql, order);
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE [Order] SET Status = @Status, CreatedDate = @CreatedDate, UpdatedDate = @UpdatedDate, ProductId = @ProductId WHERE Id = @Id";
                await connection.ExecuteAsync(sql, order);
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "DELETE FROM [Order] WHERE Id = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByFilterAsync(int? year, int? month, string status, int? productId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "SELECT * FROM [Order] o JOIN Product p ON o.ProductId = p.Id WHERE (@Year IS NULL OR YEAR(o.CreatedDate) = @Year) AND (@Month IS NULL OR MONTH(o.CreatedDate) = @Month) AND (@Status IS NULL OR o.Status = @Status) AND (@ProductId IS NULL OR o.ProductId = @ProductId)";
                var orderDictionary = new Dictionary<int, Order>();

                var orders = await connection.QueryAsync<Order, Product, Order>(sql, (order, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.Product = product;
                        orderDictionary.Add(orderEntry.Id, orderEntry);
                    }
                    return orderEntry;
                }, new { Year = year, Month = month, Status = status, ProductId = productId });

                return orders.Distinct().ToList();
            }
        }

        public async Task DeleteOrdersByFilterAsync(int? year, int? month, string status, int? productId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "DELETE FROM [Order] WHERE (@Year IS NULL OR YEAR(CreatedDate) = @Year) AND (@Month IS NULL OR MONTH(CreatedDate) = @Month) AND (@Status IS NULL OR Status = @Status) AND (@ProductId IS NULL OR ProductId = @ProductId)";
                        await connection.ExecuteAsync(sql, new { Year = year, Month = month, Status = status, ProductId = productId }, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}

