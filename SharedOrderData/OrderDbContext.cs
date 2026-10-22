using Microsoft.EntityFrameworkCore;

namespace SharedOrderData
{
    public class OrderDbContext : DbContext  //klassen ärver från EF Cores DbContext (och får all databaslogik på köpet).
    {
        public DbSet<Order> Orders => Set<Order>(); //databasen får en tabell Orders med Order-objekt. (Här EF Core får veta att Order-klassen finns)

        public static string DbPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "orders.db");                                               //DbPath anger var databasfilen ska ligga
                                                                        //Environment.GetFolderPath... :Frågar operativsystemet var den
                                                                        //inloggade användarens mapp för programdata finns.

        protected override void OnConfiguring(DbContextOptionsBuilder options)  //talar om att databasen är SQLite och var filen finns.
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }
}