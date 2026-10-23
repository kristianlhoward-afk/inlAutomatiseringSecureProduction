using SharedOrderData;  //gör att ITsystem hittar OrderDbContext och Order

namespace ITsystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var orderService = new OrderService();  // skapar databasen om den inte finns (se konstruktorn i OrderService)

            Console.WriteLine($"Databas: {OrderDbContext.DbPath}");  //skriver ut var databasfilen hamnar

            while (true)   // menyn visas om och om igen tills 0 väljs
            {
                Console.WriteLine();
                Console.WriteLine("=== Orderhantering ===");
                Console.WriteLine("1. Lista ordrar");
                Console.WriteLine("2. Skapa ny order");
                Console.WriteLine("0. Avsluta");
                Console.Write("Val: ");

                string? choice = Console.ReadLine(); //? efter string : för att inte få varning om null returneras

                switch (choice)
                {
                    case "1":
                        PrintOrders(orderService);
                        break;
                    case "2":
                        Console.WriteLine("(Skapa order - kommer i nästa steg)");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ogiltigt val.");
                        break;
                }
            }
        }

        static void PrintOrders(OrderService service)  // sköter bara utskriften, data hämtas via OrderService
        {
            var orders = service.GetAllOrders();

            if (orders.Count == 0)
            {
                Console.WriteLine("Inga ordrar finns.");
                return;
            }

            foreach (var o in orders)
            {
                Console.WriteLine($"{o.Id,4} | {o.CreatedAt:yyyy-MM-dd HH:mm} | {o.CustomerName,-20} | {o.Product,-20} | {o.Quantity,4} st | {o.Status}");
            }
        }
    }
}