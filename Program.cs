using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Client
{
    private readonly string server;
    private readonly int port;
    private readonly string authToken = "securetoken"; // Simple token-based authentication

    public Client(string server, int port)
    {
        this.server = server;
        this.port = port;
    }

    private async Task<string> SendRequest(string request)
    {
        try
        {
            using (var client = new TcpClient(server, port))
            using (var stream = client.GetStream())
            {
                var requestBytes = Encoding.UTF8.GetBytes(request);
                await stream.WriteAsync(requestBytes, 0, requestBytes.Length);
                Console.WriteLine($"Sent request: {request}");

                var buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received response: {response}");

                return response;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }

    private async Task RunClient()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("1. Register User");
        Console.WriteLine("2. Create Object");
        Console.WriteLine("3. Increment Object");
        Console.WriteLine("4. Decrement Object");
        Console.WriteLine("5. Show Status");
        Console.WriteLine("6. Exit");

        while (true)
        {
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            if (choice == "6") break;

            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            string objId = null;
            double weight = 1.0;

            if (choice != "1")
            {
                Console.Write("Enter object ID: ");
                objId = Console.ReadLine();

                if (choice == "3" || choice == "4")
                {
                    Console.Write("Enter weight (default is 1.0): ");
                    weight = double.Parse(Console.ReadLine());
                }
            }

            string request = $"{authToken} ";

            switch (choice)
            {
                case "1":
                    request += $"register {username} {password}";
                    break;
                case "2":
                    request += $"create {username} {password} {objId} {weight}";
                    break;
                case "3":
                    request += $"inc {username} {password} {objId} {weight}";
                    break;
                case "4":
                    request += $"dec {username} {password} {objId} {weight}";
                    break;
                case "5":
                    request += $"status {username} {password} {objId}";
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    continue;
            }

            string response = await SendRequest(request);
            Console.WriteLine(response);
        }
    }

    static async Task Main(string[] args)
    {
        var client = new Client("127.0.0.1", 12345);
        await client.RunClient();

        // Add this line to keep the console window open
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}
