namespace SharedOrderData
{
    public enum OrderStatus   //public så att ITsystem och IntegrationSystem kan se den             
    {
        New,                //enum kan bara ha ett begränsat antal namngivna värden (här kan
                            //en orders status bara vara New eller SentToOT (heltal: New = 0 och SentToOT = 1))
        SentToOT
    }

    public class Order  //public så att ITsystem och IntegrationSystem kan se den
    {
        public int Id { get; set; }    // EF Core hittar Id automatiskt och gör till primärnyckel som
                                       // räknas upp av databasen. (samma id som integrationen skickar till OT via Modbus)
        public string CustomerName { get; set; } = ""; //= "" ger textfälten ett startvärde. Projektet har Nullable påslaget,
                                                       //så utan startvärdet fås varningar om att fälten kan vara null.
        public string Product { get; set; } = "";
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.New;  //Status gör automatiseringen möjlig. Integrationen hämtar
                                                                    //bara ordrar med status New, och när en order är skickad ändras
                                                                    //status till SentToOT (= då skickas samma order aldrig två gånger)
    }
}