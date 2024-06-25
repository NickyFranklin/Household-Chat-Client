/*
Creator: Nicky Franklin
Date: 6/25/24
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

//byte[] bytes = new byte[1024];

try {
    //Basic Boilerplate TCP connection
    IPHostEntry host = Dns.GetHostEntry("localhost");
    IPAddress ipAddr = host.AddressList[0];
    IPEndPoint remoteEndPoint = new IPEndPoint(ipAddr, assignPort());
    Socket handler = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    try {
        //Connect to the server
        handler.Connect(remoteEndPoint);
        Console.WriteLine("Socket connected to server");

        //Where all the commands happen. This loop sends messages to the server until the user wants to leave it
        while(true) {
            //Send a message to the server
            Console.Write("Enter a command to send: ");
            string message = sendMessage(handler);

            //read a return message from the server
            bool msgReceived = false;
            var buffer = readMessage(handler, ref msgReceived);

            //When a message is received, output to user
            if(msgReceived) {
                Console.WriteLine($"Server: {String.Join("", System.Text.Encoding.ASCII.GetString(buffer.ToArray()).ToCharArray())}");
            }


            
            if(message.Equals("write")) {
                Console.Write("Enter a message to send: ");
                message = sendMessage(handler);

                //read a return message from the server
                outputMessage(handler, ref msgReceived, buffer);
            }

            else if(message.Equals("help")) {
                Console.Write("Commands:\n\twrite: tell the server you want to send a message\n\thelp: get a list of commands\n\tleave: leave the program\n");
            }

            //leave the program
            else if(message.Equals("leave")) {
                break;
            }

        }
        
        //Shutdown the socket
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

//return the port the user entered
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

//Send a message to the server and return the message that was sent
string sendMessage(Socket handler) {
    string? message = Console.ReadLine();
    #pragma warning disable CS8604 // Possible null reference argument.
        byte[] msg = Encoding.ASCII.GetBytes(message);
    #pragma warning restore CS8604 // Possible null reference argument.
    int bytesSent = handler.Send(msg);

    return message;
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

//Output the message
void outputMessage(Socket handler, ref bool msgReceived, List<byte> buffer) {
    msgReceived = false;
    buffer = readMessage(handler, ref msgReceived);
    Console.WriteLine($"Server: {String.Join("", System.Text.Encoding.ASCII.GetString(buffer.ToArray()).ToCharArray())}");
}