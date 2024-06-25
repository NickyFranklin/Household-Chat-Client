/*
Creator: Nicky Franklin
Date: 6/25/24
*/

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
    listener.Listen(1024);

    Console.WriteLine("Waiting for a connection");


    Socket handler = listener.Accept();

    while(true) {
        bool msgReceived = false;   
        var buffer = readMessage(handler, ref msgReceived);

        if(msgReceived) {
            string message = String.Join("", System.Text.Encoding.ASCII.GetString(buffer.ToArray()).ToCharArray());
            Console.WriteLine($"Here is the command that was received: {message}");
        

            if(message.Equals("leave")) {
                sendMessage(handler, "104 LEAVING");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                break;
            }

            //Dont care if help gets sent
            else if(message.Equals("help")) {
                Console.Write("Commands:\n\twrite: tell the server you want to send a message\n\thelp: get a list of commands\n\tleave: leave the program\n");
                sendMessage(handler, "312 HELP PRINTED");
            }

            //let the user send a message
            else if(message.Equals("write")) {
                sendMessage(handler, "205 WRITE START");
                buffer = readMessage(handler, ref msgReceived);
                sendMessage(handler, "200 OK");
                message = String.Join("", System.Text.Encoding.ASCII.GetString(buffer.ToArray()).ToCharArray());
                Console.WriteLine($"Here is the Message that was received: {message}");
            }

            else {
                sendMessage(handler, "505 INVALID COMMAND: TYPE help FOR A LIST OF COMMANDS");
            }
        }
    }

}

catch (Exception e) {
    Console.WriteLine(e.ToString());
}


//Read a message from the server and return it to the user
List<byte> readMessage(Socket handler, ref bool msgReceived) {
    List<byte> buffer = new();

    byte[] currByte = new byte[1];
    int byteCounter = handler.Receive(currByte, currByte.Length, SocketFlags.None);
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

    return buffer;
}

//Send a message to the server and return the message that was sent
string sendMessage(Socket handler, string message) {
    #pragma warning disable CS8604 // Possible null reference argument.
        byte[] msg = Encoding.ASCII.GetBytes(message);
    #pragma warning restore CS8604 // Possible null reference argument.
    int bytesSent = handler.Send(msg);

    return message;
}
