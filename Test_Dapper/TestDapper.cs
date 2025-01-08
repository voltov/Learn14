using Task_Dapper;

namespace Test_Dapper
{
    public class Tests
    {
        private IDbConnectionFactory _connectionFactory;
        private ProductRepository _productRepository;
        private OrderRepository _orderRepository;
        private string _connectionString = "Server=localhost;Database=plsSetSeparateDB;User Id=plsSetUser;Password=plsSetPassword;TrustServerCertificate=True;";

        [SetUp]
        public void Setup()
        {
            _connectionFactory = new SqlConnectionFactory(_connectionString);
            _productRepository = new ProductRepository(_connectionFactory);
            _orderRepository = new OrderRepository(_connectionFactory);

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    IF OBJECT_ID('dbo.Order', 'U') IS NOT NULL DROP TABLE [Order];
                    IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL DROP TABLE Product;
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
                    );";
                command.ExecuteNonQuery();
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    IF OBJECT_ID('dbo.Order', 'U') IS NOT NULL DROP TABLE [Order];
                    IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL DROP TABLE Product;";
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public async Task AddProductAsync_ShouldAddProduct()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };

            await _productRepository.AddProductAsync(product);

            var products = await _productRepository.GetAllProductsAsync();
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.First().Name, Is.EqualTo("Test Product"));
            Assert.That(products.First().Description, Is.EqualTo("Test Description"));
            Assert.That(products.First().Weight, Is.EqualTo(1.5m));
            Assert.That(products.First().Height, Is.EqualTo(10.0m));
            Assert.That(products.First().Width, Is.EqualTo(5.0m));
            Assert.That(products.First().Length, Is.EqualTo(20.0m));
        }

        [Test]
        public async Task AddOrderAsync_ShouldAddOrder()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await _productRepository.AddProductAsync(product);

            var createdProduct = (await _productRepository.GetAllProductsAsync()).First();

            var order = new Order
            {
                Status = "Not Started",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = createdProduct.Id
            };

            await _orderRepository.AddOrderAsync(order);

            var orders = await _orderRepository.GetAllOrdersAsync();
            Assert.That(orders.Count(), Is.EqualTo(1));
            Assert.That(orders.First().Status, Is.EqualTo("Not Started"));
            Assert.That(orders.First().ProductId, Is.EqualTo(createdProduct.Id));
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await _productRepository.AddProductAsync(product);

            var createdProduct = (await _productRepository.GetAllProductsAsync()).First();

            createdProduct.Name = "Updated Product";
            createdProduct.Description = "Updated Description";
            await _productRepository.UpdateProductAsync(createdProduct);

            var updatedProduct = await _productRepository.GetProductByIdAsync(createdProduct.Id);
            Assert.That(updatedProduct.Name, Is.EqualTo("Updated Product"));
            Assert.That(updatedProduct.Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public async Task UpdateOrderAsync_ShouldUpdateOrder()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await _productRepository.AddProductAsync(product);

            var createdProduct = (await _productRepository.GetAllProductsAsync()).First();

            var order = new Order
            {
                Status = "Not Started",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = createdProduct.Id
            };
            await _orderRepository.AddOrderAsync(order);

            var createdOrder = (await _orderRepository.GetAllOrdersAsync()).First();

            createdOrder.Status = "In Progress";
            await _orderRepository.UpdateOrderAsync(createdOrder);

            var updatedOrder = await _orderRepository.GetOrderByIdAsync(createdOrder.Id);
            Assert.That(updatedOrder.Status, Is.EqualTo("In Progress"));
        }

        [Test]
        public async Task DeleteProductAsync_ShouldDeleteProduct()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await _productRepository.AddProductAsync(product);

            await _productRepository.DeleteProductAsync(product.Id);

            var deletedProduct = await _productRepository.GetProductByIdAsync(product.Id);
            Assert.IsNull(deletedProduct);
        }

        [Test]
        public async Task DeleteOrderAsync_ShouldDeleteOrder()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await _productRepository.AddProductAsync(product);

            var createdProduct = (await _productRepository.GetAllProductsAsync()).First();

            var order = new Order
            {
                Status = "Not Started",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = createdProduct.Id
            };
            await _orderRepository.AddOrderAsync(order);

            await _orderRepository.DeleteOrderAsync(order.Id);

            var deletedOrder = await _orderRepository.GetOrderByIdAsync(order.Id);
            Assert.IsNull(deletedOrder);
        }

        [Test]
        public async Task GetOrdersByFilterAsync_ShouldReturnFilteredOrders()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await _productRepository.AddProductAsync(product);

            var createdProduct = (await _productRepository.GetAllProductsAsync()).First();

            var order1 = new Order
            {
                Status = "Not Started",
                CreatedDate = new DateTime(2023, 1, 1),
                UpdatedDate = DateTime.Now,
                ProductId = createdProduct.Id
            };
            var order2 = new Order
            {
                Status = "In Progress",
                CreatedDate = new DateTime(2023, 2, 1),
                UpdatedDate = DateTime.Now,
                ProductId = createdProduct.Id
            };
            await _orderRepository.AddOrderAsync(order1);
            await _orderRepository.AddOrderAsync(order2);

            var filteredOrders = await _orderRepository.GetOrdersByFilterAsync(2023, 1, "Not Started", createdProduct.Id);
            Assert.That(filteredOrders.Count(), Is.EqualTo(1));
            Assert.That(filteredOrders.First().Status, Is.EqualTo("Not Started"));
            Assert.That(filteredOrders.First().CreatedDate.Year, Is.EqualTo(2023));
            Assert.That(filteredOrders.First().CreatedDate.Month, Is.EqualTo(1));
            Assert.That(filteredOrders.First().ProductId, Is.EqualTo(createdProduct.Id));
        }

        [Test]
        public async Task DeleteOrdersByFilterAsync_ShouldDeleteFilteredOrders()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await _productRepository.AddProductAsync(product);

            var createdProduct = (await _productRepository.GetAllProductsAsync()).First();

            var order1 = new Order
            {
                Status = "Not Started",
                CreatedDate = new DateTime(2023, 1, 1),
                UpdatedDate = DateTime.Now,
                ProductId = createdProduct.Id
            };
            var order2 = new Order
            {
                Status = "In Progress",
                CreatedDate = new DateTime(2023, 2, 1),
                UpdatedDate = DateTime.Now,
                ProductId = createdProduct.Id
            };
            await _orderRepository.AddOrderAsync(order1);
            await _orderRepository.AddOrderAsync(order2);

            await _orderRepository.DeleteOrdersByFilterAsync(2023, 1, "Not Started", createdProduct.Id);

            var remainingOrders = await _orderRepository.GetAllOrdersAsync();
            Assert.That(remainingOrders.Count(), Is.EqualTo(1));
            Assert.That(remainingOrders.First().Status, Is.EqualTo("In Progress"));
        }
    }
}


