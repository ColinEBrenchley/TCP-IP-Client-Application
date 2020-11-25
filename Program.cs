using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Program
    {
        //Constant Declarations
        const string CREATE_RECORD = "CREATE_RECORD";
        const string UPDATE_RECORD = "UPDATE_RECORD";
        const string DELETE_RECORD = "DELETE_RECORD";

        const string SUCCESS = "SUCCESS";
        const string FAIL = "FAIL";

        const int BUFFER_SIZE = 100;

        static void Main(string[] args)
        {
            
            Console.WriteLine("Hello from the client...");

            const string IP_ADDRESS = "127.0.0.1"; //Localhost
            const int PORT = 50010;

            Console.Write("Enter 1: CREATE RECORD 2: UPDATE RECORD 3: DELETE RECORD 4: QUIT PROGRAM -> ");
            string response = Console.ReadLine();

            while (response != "4")
            {
                Console.WriteLine("Connecting to " + IP_ADDRESS);

                IPAddress address = IPAddress.Parse(IP_ADDRESS);
                EndPoint endPoint = new IPEndPoint(address, PORT);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);

                SendMessageToServer(socket, response);

                //Server sends back a response of either SUCCESS or FAIL
                //Receive the server's response
                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesRead = socket.Receive(buffer);

                string result = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Result of send operation: " + result);

                //SUCCESS: Client sends a record of information. Each record has the following fields:
                //  1.CLIENT_ID: An integer value unique to each client.
                //  2.PRODUCT_ID: An integer value that represents a product.
                //  3.PRODUCT_NAME: An ASCII string value that represents the name of the product.
                //  4.PRODUCT_QUANTITY: An integer value that represents the quantity.
                //  5.PRODUCT_PRICE: A decimal value that represents the price of the product.
                if (result == SUCCESS)
                {
                   if(response == "1")
                    {
                        string message = string.Empty;
                        string overwrite = string.Empty;
                        string record = string.Empty;
                        record = userInputsString();
                        
                        Console.WriteLine(record);

                        SendProductDataToServer(socket, record);

                        byte[] newBuffer = new byte[BUFFER_SIZE];
                        int newBytesRead = socket.Receive(newBuffer);

                        overwrite = Encoding.ASCII.GetString(newBuffer, 0, newBytesRead);

                        if(overwrite == "1")
                        {
                            string clientResponse = string.Empty;
                            message = "Do you want to overwrite this record? Type Y or N:";
                            Console.WriteLine(message);

                            clientResponse = Console.ReadLine();

                            SendProductDataToServer(socket, clientResponse);

                            break;

                        }
                    }

                    //2: UPDATE RECORD 
                    //  A: Prompt the user for the PRODUCT ID and the data that needs to be updated:
                    //      PRODUCT NAME? PRODUCT QUANITY?
                    else if (response == "2")
                    {
                        string message = string.Empty;
                        string overwrite = string.Empty;
                        string record = String.Empty;
                        record = userInputsString();

                        Console.WriteLine(record);

                        SendProductDataToServer(socket, record);

                        byte[] newBuffer = new byte[BUFFER_SIZE];
                        int newBytesRead = socket.Receive(newBuffer);

                        overwrite = Encoding.ASCII.GetString(newBuffer, 0, newBytesRead);

                        if (overwrite == "0")
                        {
                            string clientResponse = string.Empty;
                            message = "No record with that Product_ID found. Do you want to create a new record? Type Y or N:";
                            Console.WriteLine(message);

                            clientResponse = Console.ReadLine();

                            SendProductDataToServer(socket, clientResponse);

                            break;
                        }
                    }

                    //3: DELETE RECORD
                    //  A: Prompt the user for the PRODUCT ID
                    else if (response == "3")
                    {
                        string record = String.Empty; 
                        record = userInputsString();

                        Console.WriteLine(record);

                        SendProductDataToServer(socket, record);
                    }

                    //Server sends back a response of either SUCCESS or FAIL
                    //Receive the server's response
                    buffer = new byte[BUFFER_SIZE];
                    bytesRead = socket.Receive(buffer);

                    result = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("Result of send operation: " + result);
                }

                //FAIL: Client notifies the user that there was an error


              socket.Close();

                Console.Write("Enter 1: CREATE RECORD 2: UPDATE RECORD 3: DELETE RECORD 4: QUIT PROGRAM -> ");
                response = Console.ReadLine();
            }

            Console.Read();
        }

        private static void SendMessageToServer(Socket connection, string recordType)
        {
            string data = string.Empty;

            if (recordType == "1")
            {
                data = CREATE_RECORD;
            }
            else if (recordType == "2")
            {
                data = UPDATE_RECORD;
            }
            else if (recordType == "3")
            {
                data = DELETE_RECORD;
            }

            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int sentBytes = connection.Send(buffer);

            Console.WriteLine("Sent " + sentBytes + " bytes." + Environment.NewLine);
        }

        private static void SendProductDataToServer(Socket connection, string record)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(record);

            int sentBytes = connection.Send(buffer);

            Console.WriteLine("Sent " + sentBytes + " bytes." + Environment.NewLine);
        }
        private static string userInputsString()
        {
            string record = string.Empty;

            Console.Write("Enter Product ID: ");
            record += Console.ReadLine();
            record += ", ";

            Console.Write("Enter Product Name: ");
            record += Console.ReadLine();
            record += ", ";

            Console.Write("Enter Product Quantity: ");
            record += Console.ReadLine();
            record += ", ";

            Console.Write("Enter Product Price: ");
            record += Console.ReadLine();

            return record;
        }
        private static void breakGlassInCaseOfEmergency(Socket socket)
        {
            string serverMessage = string.Empty;
            string clientResponse = string.Empty;

            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesRead = socket.Receive(buffer);
            serverMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            Console.WriteLine(serverMessage);
            clientResponse = Console.ReadLine();

            SendProductDataToServer(socket, clientResponse);

            return;
        }
    }
}
