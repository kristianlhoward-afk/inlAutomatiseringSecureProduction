using SharedOrderData;

namespace ITsystem
{
    public class OrderService
    {
        public OrderService()  // konstruktorn körs automatiskt vid new OrderService()
        {
            using var db = new OrderDbContext();
            db.Database.EnsureCreated();   // skapar databasen om den inte redan finns.
        }

        public List<Order> GetAllOrders()
        {
            using var db = new OrderDbContext();
            return db.Orders.OrderBy(o => o.Id).ToList();  // ToList hämtar datan innan kopplingen stängs
        }
    }
}