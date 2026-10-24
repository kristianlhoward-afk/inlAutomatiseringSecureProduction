using EasyModbus;

namespace IntegrationSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var orderService = new IntegrationOrderService();
            var modbusClient = new ModbusClient("127.0.0.1", 502);

            Console.WriteLine("Integrationssystem startat. Bevakar nya ordrar...");

            while (true)
            {
                try
                {
                    if (!modbusClient.Connected)
                    {
                        modbusClient.Connect();
                        Console.WriteLine("Ansluten till OT-systemet.");
                    }

                    modbusClient.ReadHoldingRegisters(0, 1);  // heartbeat: kastar fel direkt om OT inte svarar

                    //Nya ordrar hämtas . Loopa läsa i databasen - för varje order
                    var newOrders = orderService.GetNewOrders();

                    foreach (var order in newOrders)
                    {
                        // Register 0 = order-id, register 1 = antal
                        modbusClient.WriteMultipleRegisters(0, new[] { order.Id, order.Quantity });//skickar både id (register 0)
                                                                                                   //och antal (register 1) i ett anrop

                        //modbusClient.WriteMultipleRegisters(0, new[] { order.Id, 5000 });  //Negativt test (fault injection)


                        orderService.MarkAsSent(order.Id);
                        Console.WriteLine($"Order {order.Id} skickad till OT ({order.Quantity} st).");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fel: {ex.Message} - försöker igen om 5 s.");
                    try { modbusClient.Disconnect(); } catch { }
                }

                Thread.Sleep(5000);
            }
        }
    }
}



