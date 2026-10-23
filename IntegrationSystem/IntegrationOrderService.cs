using SharedOrderData;

namespace IntegrationSystem
{
    public class IntegrationOrderService
    {
        public IntegrationOrderService()
        {
            using var db = new OrderDbContext();
            db.Database.EnsureCreated();   //Integrationen kan startas innan ITsystem har körts - då måste databasen finnas redan
        }

        public List<Order> GetNewOrders()  //hämtar ordrar...
        {
            using var db = new OrderDbContext();  
            return db.Orders
                .Where(o => o.Status == OrderStatus.New)  //... med status New
                .OrderBy(o => o.Id)
                .ToList();
        }

        public void MarkAsSent(int orderId)   //letar upp order med Find (sökning på primärnyckeln)....
        {
            using var db = new OrderDbContext();
            var order = db.Orders.Find(orderId);

            if (order != null)
            {
                order.Status = OrderStatus.SentToOT; //  .....och ändrar status till SentToOT
                db.SaveChanges();
            }
        }
    }
}