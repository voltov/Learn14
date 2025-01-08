using Microsoft.EntityFrameworkCore;

namespace Task_Entity
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.Product).ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByFilterAsync(int? year, int? month, string status, int? productId)
        {
            var query = _context.Orders.AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(o => o.CreatedDate.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(o => o.CreatedDate.Month == month.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (productId.HasValue)
            {
                query = query.Where(o => o.ProductId == productId.Value);
            }

            return await query.Include(o => o.Product).ToListAsync();
        }

        public async Task DeleteOrdersByFilterAsync(int? year, int? month, string status, int? productId)
        {
            var orders = await GetOrdersByFilterAsync(year, month, status, productId);
            _context.Orders.RemoveRange(orders);
            await _context.SaveChangesAsync();
        }
    }
}
