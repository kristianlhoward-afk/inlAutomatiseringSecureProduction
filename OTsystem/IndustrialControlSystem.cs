using EasyModbus;
using System;
using System.Linq; // For .Cast<T>() and .Skip().Take() in event handlers
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace OTsystem
{
    internal class IndustrialControlSystem
    {
        private static double currentTemperature = 20.0;
        private const double TargetTemperature = 25.0;
        private static bool heaterOn = false;
        private static bool messageReceived = false;

        public void Run()
        {
            Console.WriteLine("Simulated OT system with Modbus support");

            Thread modbusThread = new Thread(StartEasyModbusTcpSlave);
            modbusThread.IsBackground = true;
            modbusThread.Start();

            while (true)
            {
                //Console.WriteLine($"Current temperature: {currentTemperature:F1}°C");
                //heaterOn = currentTemperature < TargetTemperature;
                //Console.WriteLine(heaterOn ? "Heater ON" : "Heater OFF");
                //currentTemperature += heaterOn ? 0.5 : -0.1;

                if (messageReceived)
                {
                    Console.WriteLine("Order received via Modbus!");
                    messageReceived = false;
                }

                Thread.Sleep(1000);
            }
        }
        public static void StartEasyModbusTcpSlave()
        {
            int port = 502;
            // IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 }); // Not directly used by EasyModbusSlave
            // EasyModbusSlave listens on all available interfaces by default if IPAddress is null or empty.
            // Or you can specify a specific IP address string.

            // Create an EasyModbusSlave instance
            // EasyModbusSlave(int listenPort, string? ipAddress = null)
            ModbusServer modbusServer = new ModbusServer();
            modbusServer.Port = port; // Set the port number
            modbusServer.LocalIPAddress = IPAddress.Loopback;  // lyssnar bara på 127.0.0.1, ej nåbar från nätverket
           

            // --- Event Handlers for EasyModbus ---

            // Event for when Coils (Digital Outputs) are written to by a client
            modbusServer.CoilsChanged += (int startAddress, int numberOfCoils) =>
            {
                Console.WriteLine($"CoilsChanged event fired at {DateTime.Now}");
                Console.WriteLine($"  Start Address: {startAddress}");
                Console.WriteLine($"  Number of Coils: {numberOfCoils}");

                // Assuming EasyModbus internal arrays for coils and holding registers
                // are of a fixed, known size (e.g., 2000).
                // If modbusServer.coils.Length doesn't work, we'll use a hardcoded max or try another property.
                const int maxCoilAddress = 1999; // Default max index if array size is 2000 (0-1999)

                for (int i = 0; i < numberOfCoils; i++)
                {
                    int address = startAddress + i;
                    // Relying on knowledge of EasyModbus's default internal array size,
                    // or if you've set custom sizes for the server's data arrays.
                    if (address >= 0 && address <= maxCoilAddress) // Ensure we don't go out of bounds
                    {
                        Console.WriteLine($"    Coil[{address}] changed to: {modbusServer.coils[address]}");
                    }
                    else
                    {
                        Console.WriteLine($"    Warning: Attempted to access Coil[{address}] which is out of bounds.");
                    }
                }
            };

            modbusServer.HoldingRegistersChanged += (int startAddress, int numberOfRegisters) =>
            {
                Console.WriteLine($"HoldingRegistersChanged event fired at {DateTime.Now}");
                Console.WriteLine($"  Start Address: {startAddress}");
                Console.WriteLine($"  Number of Registers: {numberOfRegisters}");

                // Assuming EasyModbus internal arrays for coils and holding registers
                // are of a fixed, known size (e.g., 2000).
                const int maxRegisterAddress = 1999; // Default max index if array size is 2000 (0-1999)

                for (int i = 0; i < numberOfRegisters; i++)
                {
                    int address = startAddress + i;
                    if (address >= 0 && address <= maxRegisterAddress) // Ensure we don't go out of bounds
                    {
                        Console.WriteLine($"    HoldingRegister[{address}] changed to: {modbusServer.holdingRegisters[address]}");
                    }
                    else
                    {
                        Console.WriteLine($"    Warning: Attempted to access HoldingRegister[{address}] which is out of bounds.");
                    }
                }


                // Validering av inkommande order innan produktion startas
                int orderId = modbusServer.holdingRegisters[startAddress];
                int quantity = modbusServer.holdingRegisters[startAddress + 1];

                if (numberOfRegisters < 2 || orderId <= 0 || quantity < 1 || quantity > 1000)
                {
                    Console.WriteLine($"AVVISAD: ogiltig order (id={orderId}, antal={quantity}) - produktion startas inte.");
                    return;
                }

                Console.WriteLine($"Startar produktion: order {orderId}, {quantity} st");
                messageReceived = true;





            };
            // EasyModbus also has events for Inputs and DiscreteInputs if you need them:
            // modbusServer.InputRegistersChanged += (sender, args) => { /* ... */ };
            // modbusServer.DiscreteInputsChanged += (sender, args) => { /* ... */ };


            // --- Pre-populate Data (Optional) ---
            // EasyModbus pre-allocates arrays for its data points.
            // Default sizes: coils[2000], discreteInputs[2000], holdingRegisters[2000], inputRegisters[2000]
            // You can set initial values directly:
            modbusServer.holdingRegisters[0] = 123;
            modbusServer.holdingRegisters[1] = 456;
            modbusServer.coils[0] = true;
            modbusServer.holdingRegisters[10] = 789; // Example for a higher address

            // You can also set specific input/discrete values (read-only for client)
            modbusServer.inputRegisters[0] = 999;
            modbusServer.discreteInputs[0] = true;


            // --- Start the Modbus Server ---
            try
            {
                Console.WriteLine($"Starting EasyModbus TCP Slave on port {port}...");
                modbusServer.Listen();

                Console.WriteLine("EasyModbus TCP Slave started. Press any key to exit.");
                Console.ReadKey(); // Keep the console open until a key is pressed

                // --- Stop the Modbus Server ---
                Console.WriteLine("Stopping EasyModbus TCP Slave...");
                Console.WriteLine("EasyModbus TCP Slave stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}