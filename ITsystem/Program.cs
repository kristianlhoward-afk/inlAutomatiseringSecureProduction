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
                        CreateOrder(orderService);
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


        static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(input) && input.Length <= 100)
                    return input;

                Console.WriteLine("Får inte vara tomt (max 100 tecken).");
            }
        }

        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);

                if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                    return value;

                Console.WriteLine($"Ange ett heltal mellan {min} och {max}.");
            }
        }

        static void CreateOrder(OrderService service)
        {
            string customer = ReadRequired("Kundnamn: ");
            string product = ReadRequired("Produkt: ");
            int quantity = ReadInt("Antal (1-1000): ", 1, 1000);

            var order = service.CreateOrder(customer, product, quantity);
            Console.WriteLine($"Order {order.Id} skapad.");
        }
    }
}
