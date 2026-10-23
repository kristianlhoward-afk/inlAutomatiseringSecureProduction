using SharedOrderData;  //gör att ITsystem hittar OrderDbContext och Order

namespace ITsystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var db = new OrderDbContext())
            {
                db.Database.EnsureCreated();   // skapar databasen om den inte redan finns.
            }

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
                        Console.WriteLine("(Lista ordrar - kommer i nästa steg)");
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
    }
}