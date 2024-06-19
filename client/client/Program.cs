using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

byte[] bytes = new byte[1024];

try {
    IPHostEntry host = Dns.GetHostEntry("localhost");
    IPAddress ipAddr = host.AddressList[0];
    IPEndPoint remoteEndPoint = new IPEndPoint(ipAddr, assignPort());

    Socket handler = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    try {
        handler.Connect(remoteEndPoint);
        Console.WriteLine("Socket connected to server");

        while(true) {
            Console.Write("Enter a message to send: ");
            string? message = Console.ReadLine();
            byte[] msg = Encoding.ASCII.GetBytes(message);
            int bytesSent = handler.Send(msg);

            var buffer = new List<byte>();
            bool msgReceived = false;

            var currByte = new byte[1];
            var byteCounter = handler.Receive(currByte, currByte.Length, SocketFlags.None);
            while(handler.Available > 0) {
                if (byteCounter.Equals(1)) {
                    buffer.Add(currByte[0]);
                }
                
                msgReceived = true;
                currByte = new byte[1];
                byteCounter = handler.Receive(currByte, currByte.Length, SocketFlags.None);

                if (handler.Available <= 0) {
                    buffer.Add(currByte[0]);
                }
            }

            if(msgReceived) {
                Console.WriteLine($"Server: {String.Join("", System.Text.Encoding.ASCII.GetString(buffer.ToArray()).ToCharArray())}");
            }

            if(message.Equals("leave")) {
                break;
            }

        }
        

        handler.Shutdown(SocketShutdown.Both);
        handler.Close();
    }

    catch (ArgumentNullException ane) {
        Console.WriteLine("ArgumentNullExcept : {0}", ane.ToString());
    }

    catch (SocketException se) {
        Console.WriteLine("Socket Exception : {0}", se.ToString());
    }

    catch (Exception e) {
        Console.WriteLine("Unexpected Exception : {0}", e.ToString());
    }
}

catch (Exception e) {
    Console.WriteLine(e.ToString());
}

int assignPort() {
    try {
        int temp = int.Parse(args[0]);
    }

    catch (IndexOutOfRangeException iore) {
        Console.WriteLine("No Arguments given, please give argument: {0}", iore.ToString());
        System.Environment.Exit(1);
    }

    catch (Exception e) {
        Console.WriteLine(e.ToString());
        System.Environment.Exit(1);
    }

    return int.Parse(args[0]);
}