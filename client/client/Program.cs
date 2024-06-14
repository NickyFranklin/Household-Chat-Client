using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

byte[] bytes = new byte[1024];

try {
    IPHostEntry host = Dns.GetHostEntry("localhost");
    IPAddress ipAddr = host.AddressList[0];
    IPEndPoint remoteEndPoint = new IPEndPoint(ipAddr, 11000);

    Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    try {
        sender.Connect(remoteEndPoint);

        Console.WriteLine("Socket connected to server");

        byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

        int bytesSent = sender.Send(msg);
        int bytesReceived = sender.Receive(bytes);
        Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesReceived));

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
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