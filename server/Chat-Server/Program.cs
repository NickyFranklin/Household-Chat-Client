using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

//Sets up struct/class with server IP and Port information
IPHostEntry host = Dns.GetHostEntry("localhost");
IPAddress ipAddr = host.AddressList[0];
IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11000); //Tutorial did 11000, I hope this just gives a random port

try {

    //Create Socket that uses TCP
    Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    //Bind Socket with server info
    listener.Bind(localEndPoint);

    //Listen to certain number of requests at once
    listener.Listen(1);

    Console.WriteLine("Waiting for a connection");
    Socket handler = listener.Accept();

    string? data = null;
    byte[]? bytes = null;

    while(true) {
        bytes = new byte[1024];
        int bytesReceived = handler.Receive(bytes);
        data += Encoding.ASCII.GetString(bytes, 0, bytesReceived);
        if(data.IndexOf("<EOF>") > -1) {
            break;
        }
    }

    Console.WriteLine("Text Received : {0}", data);

    byte[] msg = Encoding.ASCII.GetBytes(data);
    handler.Send(msg);
    handler.Shutdown(SocketShutdown.Both);
    handler.Close();
}

catch (Exception e) {
    Console.WriteLine(e.ToString());
}

Console.WriteLine("\nPress any key to continue");
Console.ReadKey();
Console.Write("\n");