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

        public Order CreateOrder(string customerName, string product, int quantity)
        {
            using var db = new OrderDbContext();
            var order = new Order
            {
                CustomerName = customerName,
                Product = product,
                Quantity = quantity
            };
            db.Orders.Add(order);   // talar om för EF Core att ordern ska läggas till
            db.SaveChanges();       // skriver till databasen, här får ordern sitt Id
            return order;
        }



    }
}