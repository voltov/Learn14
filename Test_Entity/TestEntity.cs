using Microsoft.EntityFrameworkCore;

namespace Task_Entity.Tests
{
    public class Tests
    {
        private DbContextOptions<AppDbContext> _options;
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var connectionString = "Server=localhost;Database=plsSetSeparateDB;User Id=plsSetUser;Password=plsSetPassword;TrustServerCertificate=True;";
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            _context = new AppDbContext(_options);

            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddProductAsync_ShouldAddProduct()
        {
            var repository = new ProductRepository(_context);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };

            await repository.AddProductAsync(product);

            var products = await _context.Products.ToListAsync();
            Assert.That(products.Count, Is.EqualTo(1));
            Assert.That(products[0].Name, Is.EqualTo("Test Product"));
            Assert.That(products[0].Description, Is.EqualTo("Test Description"));
            Assert.That(products[0].Weight, Is.EqualTo(1.5m));
            Assert.That(products[0].Height, Is.EqualTo(10.0m));
            Assert.That(products[0].Width, Is.EqualTo(5.0m));
            Assert.That(products[0].Length, Is.EqualTo(20.0m));
        }

        [Test]
        public async Task AddOrderAsync_ShouldAddOrder()
        {
            var productRepository = new ProductRepository(_context);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await productRepository.AddProductAsync(product);

            var orderRepository = new OrderRepository(_context);
            var order = new Order
            {
                Status = "Not Started",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };

            await orderRepository.AddOrderAsync(order);

            var orders = await _context.Orders.Include(o => o.Product).ToListAsync();
            Assert.That(orders.Count, Is.EqualTo(1));
            Assert.That(orders[0].Status, Is.EqualTo("Not Started"));
            Assert.That(orders[0].ProductId, Is.EqualTo(product.Id));
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            var repository = new ProductRepository(_context);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await repository.AddProductAsync(product);

            product.Name = "Updated Product";
            product.Description = "Updated Description";
            await repository.UpdateProductAsync(product);

            var updatedProduct = await _context.Products.FindAsync(product.Id);
            Assert.That(updatedProduct.Name, Is.EqualTo("Updated Product"));
            Assert.That(updatedProduct.Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public async Task UpdateOrderAsync_ShouldUpdateOrder()
        {
            var productRepository = new ProductRepository(_context);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await productRepository.AddProductAsync(product);

            var orderRepository = new OrderRepository(_context);
            var order = new Order
            {
                Status = "Not Started",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };
            await orderRepository.AddOrderAsync(order);

            order.Status = "In Progress";
            await orderRepository.UpdateOrderAsync(order);

            var updatedOrder = await _context.Orders.FindAsync(order.Id);
            Assert.That(updatedOrder.Status, Is.EqualTo("In Progress"));
        }

        [Test]
        public async Task DeleteProductAsync_ShouldDeleteProduct()
        {
            var repository = new ProductRepository(_context);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await repository.AddProductAsync(product);

            await repository.DeleteProductAsync(product.Id);

            var deletedProduct = await _context.Products.FindAsync(product.Id);
            Assert.IsNull(deletedProduct);
        }

        [Test]
        public async Task DeleteOrderAsync_ShouldDeleteOrder()
        {
            var productRepository = new ProductRepository(_context);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await productRepository.AddProductAsync(product);

            var orderRepository = new OrderRepository(_context);
            var order = new Order
            {
                Status = "Not Started",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };
            await orderRepository.AddOrderAsync(order);

            await orderRepository.DeleteOrderAsync(order.Id);

            var deletedOrder = await _context.Orders.FindAsync(order.Id);
            Assert.IsNull(deletedOrder);
        }

        [Test]
        public async Task GetOrdersByFilterAsync_ShouldReturnFilteredOrders()
        {
            var productRepository = new ProductRepository(_context);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await productRepository.AddProductAsync(product);

            var orderRepository = new OrderRepository(_context);
            var order1 = new Order
            {
                Status = "Not Started",
                CreatedDate = new DateTime(2023, 1, 1),
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };
            var order2 = new Order
            {
                Status = "In Progress",
                CreatedDate = new DateTime(2023, 2, 1),
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };
            await orderRepository.AddOrderAsync(order1);
            await orderRepository.AddOrderAsync(order2);

            var filteredOrders = await orderRepository.GetOrdersByFilterAsync(2023, 1, "Not Started", product.Id);
            Assert.That(filteredOrders.Count(), Is.EqualTo(1));
            Assert.That(filteredOrders.First().Status, Is.EqualTo("Not Started"));
            Assert.That(filteredOrders.First().CreatedDate.Year, Is.EqualTo(2023));
            Assert.That(filteredOrders.First().CreatedDate.Month, Is.EqualTo(1));
            Assert.That(filteredOrders.First().ProductId, Is.EqualTo(product.Id));
        }

        [Test]
        public async Task DeleteOrdersByFilterAsync_ShouldDeleteFilteredOrders()
        {
            var productRepository = new ProductRepository(_context);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Weight = 1.5m,
                Height = 10.0m,
                Width = 5.0m,
                Length = 20.0m
            };
            await productRepository.AddProductAsync(product);

            var orderRepository = new OrderRepository(_context);
            var order1 = new Order
            {
                Status = "Not Started",
                CreatedDate = new DateTime(2023, 1, 1),
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };
            var order2 = new Order
            {
                Status = "In Progress",
                CreatedDate = new DateTime(2023, 2, 1),
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };
            await orderRepository.AddOrderAsync(order1);
            await orderRepository.AddOrderAsync(order2);

            await orderRepository.DeleteOrdersByFilterAsync(2023, 1, "Not Started", product.Id);

            var remainingOrders = await _context.Orders.ToListAsync();
            Assert.That(remainingOrders.Count, Is.EqualTo(1));
            Assert.That(remainingOrders.First().Status, Is.EqualTo("In Progress"));
        }

    }
}
