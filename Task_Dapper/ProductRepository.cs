using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Task_Dapper
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Product>("SELECT * FROM Product");
            }
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Product WHERE Id = @Id", new { Id = id });
            }
        }

        public async Task AddProductAsync(Product product)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "INSERT INTO Product (Name, Description, Weight, Height, Width, Length) VALUES (@Name, @Description, @Weight, @Height, @Width, @Length)";
                await connection.ExecuteAsync(sql, product);
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE Product SET Name = @Name, Description = @Description, Weight = @Weight, Height = @Height, Width = @Width, Length = @Length WHERE Id = @Id";
                await connection.ExecuteAsync(sql, product);
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "DELETE FROM Product WHERE Id = @Id";
                await connection.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}