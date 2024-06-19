using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

//Sets up struct/class with server IP and Port information
IPHostEntry host = Dns.GetHostEntry("localhost");
IPAddress ipAddr = host.AddressList[0];
IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 0); //Tutorial did 11000, I hope this just gives a random port

try {
    //Create Socket that uses TCP
    Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    //Bind Socket with server info
    listener.Bind(localEndPoint);
    Console.WriteLine("Port " + ((IPEndPoint)listener.LocalEndPoint).Port);

    //Listen to certain number of requests at once
    listener.Listen(1);

    Console.WriteLine("Waiting for a connection");
    Socket handler = listener.Accept();

    while(true) {
        var buffer = new List<byte>();    
        bool msgReceived = false;    
        while(handler.Available > 0) {
            var currByte = new byte[1];
            var byteCounter = handler.Receive(currByte, currByte.Length, SocketFlags.None);

            if (byteCounter.Equals(1)) {
                buffer.Add(currByte[0]);
            }
            
            msgReceived = true;
        }

        if(msgReceived) {
            string message = String.Join("", System.Text.Encoding.ASCII.GetString(buffer.ToArray()).ToCharArray());
            Console.WriteLine($"Here is the Message that was received: {message}");
        

            if(message.Equals("leave")) {
                byte[] msg = Encoding.ASCII.GetBytes("104 LEAVING");
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                break;
            }

            else {
                byte[] msg = Encoding.ASCII.GetBytes("200 OK");
                handler.Send(msg);
            }
        }
    }

}

catch (Exception e) {
    Console.WriteLine(e.ToString());
}

